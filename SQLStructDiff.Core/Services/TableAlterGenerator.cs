using System.Text;
using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>
/// Gera comandos ALTER TABLE para tornar a tabela ALVO igual à tabela ORIGEM,
/// comparando coluna a coluna (ADD / DROP / ALTER COLUMN + ajuste de DEFAULT).
/// </summary>
public sealed class TableAlterGenerator
{
    /// <summary>
    /// Gera os comandos para alterar <paramref name="target"/> de modo a refletir
    /// a estrutura de <paramref name="source"/>. Retorna linhas separadas por GO.
    /// </summary>
    public string Generate(DbObject source, DbObject target)
    {
        var fullName = $"[{target.Schema}].[{target.Name}]";
        var sourceCols = source.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        var targetCols = target.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();

        // 1) Colunas removidas no alvo (existem no alvo, não na origem) -> DROP COLUMN
        foreach (var col in target.Columns.Where(c => !sourceCols.ContainsKey(c.Name)))
        {
            if (col.DefaultConstraintName is not null)
                AppendBatch(sb, $"ALTER TABLE {fullName} DROP CONSTRAINT [{col.DefaultConstraintName}];");
            AppendBatch(sb, $"ALTER TABLE {fullName} DROP COLUMN [{col.Name}];");
        }

        // 2) Colunas novas (existem na origem, não no alvo) -> ADD
        foreach (var col in source.Columns.Where(c => !targetCols.ContainsKey(c.Name)).OrderBy(c => c.Ordinal))
        {
            if (col.IsIdentity)
                sb.AppendLine($"-- ATENÇÃO: coluna IDENTITY [{col.Name}] adicionada; revise o impacto.");
            AppendBatch(sb, $"ALTER TABLE {fullName} ADD {SchemaExtractor.ColumnDdl(col, indent: false)};");
        }

        // 3) Colunas presentes nos dois: detectar diferença de tipo/nulabilidade e de default
        foreach (var srcCol in source.Columns.Where(c => targetCols.ContainsKey(c.Name)).OrderBy(c => c.Ordinal))
        {
            var tgtCol = targetCols[srcCol.Name];
            EmitColumnChange(sb, fullName, srcCol, tgtCol);
        }

        return sb.ToString();
    }

    private static void EmitColumnChange(StringBuilder sb, string fullName, TableColumn src, TableColumn tgt)
    {
        // Identity não pode ser alterado via ALTER COLUMN.
        if (src.IsIdentity != tgt.IsIdentity)
            sb.AppendLine($"-- ATENÇÃO: alteração de IDENTITY na coluna [{src.Name}] exige recriar a coluna/tabela (não gerado).");

        // Tipo ou nulabilidade diferentes -> ALTER COLUMN
        if (src.TypeSignature != tgt.TypeSignature)
        {
            var nullability = src.IsNullable ? "NULL" : "NOT NULL";
            AppendBatch(sb, $"ALTER TABLE {fullName} ALTER COLUMN [{src.Name}] {src.TypeDefinition} {nullability};");
        }

        // Default diferente -> dropar o do alvo (se houver) e adicionar o da origem (se houver)
        if (!string.Equals(src.DefaultDefinition, tgt.DefaultDefinition, StringComparison.OrdinalIgnoreCase))
        {
            if (tgt.DefaultConstraintName is not null)
                AppendBatch(sb, $"ALTER TABLE {fullName} DROP CONSTRAINT [{tgt.DefaultConstraintName}];");

            if (src.DefaultDefinition is not null)
            {
                var name = src.DefaultConstraintName ?? $"DF_{TrimName(fullName)}_{src.Name}";
                AppendBatch(sb,
                    $"ALTER TABLE {fullName} ADD CONSTRAINT [{name}] DEFAULT {src.DefaultDefinition} FOR [{src.Name}];");
            }
        }
    }

    private static string TrimName(string fullName) =>
        fullName.Replace("[", "").Replace("]", "").Replace(".", "_");

    private static void AppendBatch(StringBuilder sb, string statement)
    {
        sb.AppendLine(statement);
        sb.AppendLine("GO");
    }
}
