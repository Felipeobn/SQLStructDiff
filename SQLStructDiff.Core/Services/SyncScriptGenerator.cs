using System.Text;
using System.Text.RegularExpressions;
using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>Direção da sincronização.</summary>
public enum SyncDirection
{
    /// <summary>Tornar B igual a A (origem A, alvo B).</summary>
    AToB,

    /// <summary>Tornar A igual a B (origem B, alvo A).</summary>
    BToA
}

/// <summary>
/// Gera o script DDL para sincronizar os objetos selecionados em uma direção.
/// O script é ordenado por dependência. Tabelas com diferenças geram um bloco
/// comentado para revisão manual (evita perda de dados por DROP/CREATE).
/// </summary>
public sealed partial class SyncScriptGenerator
{
    [GeneratedRegex(@"^\s*CREATE\s+(VIEW|PROCEDURE|PROC|TRIGGER)\b", RegexOptions.IgnoreCase)]
    private static partial Regex CreateModule();

    public string Generate(IEnumerable<ObjectComparison> selected, SyncDirection direction,
        string? sourceName = null, string? targetName = null)
    {
        var items = selected.Where(c => c.Status != CompareStatus.Equal).ToList();
        var sb = new StringBuilder();

        // Usa os nomes reais dos bancos quando informados; senão cai no rótulo A/B.
        sourceName ??= direction == SyncDirection.AToB ? "A" : "B";
        targetName ??= direction == SyncDirection.AToB ? "B" : "A";

        sb.AppendLine("-- ============================================================");
        sb.AppendLine($"-- Script de sincronização ({sourceName} -> {targetName})");
        sb.AppendLine("-- Gerado por SQLStructDiff. Revise antes de executar.");
        sb.AppendLine("-- ============================================================");
        sb.AppendLine("SET XACT_ABORT ON;  -- qualquer erro em runtime aborta e faz rollback");
        sb.AppendLine("GO");
        sb.AppendLine("BEGIN TRANSACTION;");
        sb.AppendLine("GO");
        sb.AppendLine();

        // Ordem para DROP: triggers -> procedures -> views -> índices -> tabelas
        EmitDrops(sb, items, direction, DbObjectType.Trigger);
        EmitDrops(sb, items, direction, DbObjectType.Procedure);
        EmitDrops(sb, items, direction, DbObjectType.View);
        EmitDrops(sb, items, direction, DbObjectType.Index);
        EmitDrops(sb, items, direction, DbObjectType.Table);

        // Ordem para CREATE/ALTER: tabelas -> índices -> views -> procedures -> triggers
        EmitCreates(sb, items, direction, DbObjectType.Table);
        EmitCreates(sb, items, direction, DbObjectType.Index);
        EmitCreates(sb, items, direction, DbObjectType.View);
        EmitCreates(sb, items, direction, DbObjectType.Procedure);
        EmitCreates(sb, items, direction, DbObjectType.Trigger);

        sb.AppendLine("COMMIT TRANSACTION;");
        sb.AppendLine("GO");

        return sb.ToString();
    }

    // Objeto presente apenas no ALVO precisa ser removido (DROP).
    private void EmitDrops(StringBuilder sb, List<ObjectComparison> items,
        SyncDirection dir, DbObjectType type)
    {
        var onlyInTarget = dir == SyncDirection.AToB ? CompareStatus.OnlyInB : CompareStatus.OnlyInA;
        foreach (var c in items.Where(c => c.Type == type && c.Status == onlyInTarget))
        {
            // Views/Procedures saem sem os marcadores de comentário da ferramenta.
            if (!IsModule(type))
                sb.AppendLine($"-- DROP {type}: {c.FullName}");
            sb.AppendLine(DropStatement(c));
            sb.AppendLine("GO");
            sb.AppendLine();
        }
    }

    // Objeto presente na ORIGEM (só na origem, ou diferente) precisa ser criado/alterado.
    private void EmitCreates(StringBuilder sb, List<ObjectComparison> items,
        SyncDirection dir, DbObjectType type)
    {
        var onlyInSource = dir == SyncDirection.AToB ? CompareStatus.OnlyInA : CompareStatus.OnlyInB;

        foreach (var c in items.Where(c => c.Type == type &&
                 (c.Status == onlyInSource || c.Status == CompareStatus.Different)))
        {
            var source = dir == SyncDirection.AToB ? c.ObjectA : c.ObjectB;
            if (source is null) continue;

            if (type == DbObjectType.Table && c.Status == CompareStatus.Different)
            {
                var target = dir == SyncDirection.AToB ? c.ObjectB : c.ObjectA;
                if (target is null) continue;

                sb.AppendLine($"-- ALTER tabela: {c.FullName} (diferenças coluna a coluna)");
                var alter = new TableAlterGenerator().Generate(source, target).TrimEnd();
                sb.AppendLine(alter.Length > 0
                    ? alter
                    : "-- (sem diferenças de coluna detectadas; verifique PK/índices)");
                sb.AppendLine();
                continue;
            }

            // Índice alterado: recria (DROP + CREATE).
            if (c.Status == CompareStatus.Different && type == DbObjectType.Index)
            {
                sb.AppendLine($"-- Recriando índice alterado: {c.FullName}");
                sb.AppendLine(DropStatement(c));
                sb.AppendLine("GO");
            }

            // Views/Procedures saem sem os marcadores de comentário da ferramenta.
            if (!IsModule(type))
                sb.AppendLine($"-- {(c.Status == CompareStatus.Different ? "ALTER" : "CREATE")} {type}: {c.FullName}");
            sb.AppendLine(CreateStatement(source, c.Status));
            sb.AppendLine("GO");
            sb.AppendLine();
        }
    }

    private string CreateStatement(DbObject source, CompareStatus status)
    {
        // Views/Procedures/Triggers alterados usam CREATE OR ALTER.
        if (source.Type is DbObjectType.View or DbObjectType.Procedure or DbObjectType.Trigger)
        {
            var ddl = source.RawDefinition;
            if (status == CompareStatus.Different && CreateModule().IsMatch(ddl))
                ddl = CreateModule().Replace(ddl, m => "CREATE OR ALTER " + m.Groups[1].Value.ToUpperInvariant(), 1);
            return ddl;
        }

        // Tabela nova / índice: usa o DDL reconstruído.
        return source.RawDefinition;
    }

    // Views, Procedures e Triggers são "módulos": seus scripts saem sem os comentários de marcação.
    private static bool IsModule(DbObjectType type) =>
        type is DbObjectType.View or DbObjectType.Procedure or DbObjectType.Trigger;

    private static string DropStatement(ObjectComparison c)
    {
        return c.Type switch
        {
            DbObjectType.Procedure => $"DROP PROCEDURE IF EXISTS {c.FullName};",
            DbObjectType.View => $"DROP VIEW IF EXISTS {c.FullName};",
            DbObjectType.Trigger => $"DROP TRIGGER IF EXISTS {c.FullName};",
            DbObjectType.Table => $"DROP TABLE IF EXISTS {c.FullName};",
            DbObjectType.Index => DropIndex(c),
            _ => $"-- DROP não suportado para {c.Type}"
        };
    }

    private static string DropIndex(ObjectComparison c)
    {
        // Name = "Tabela.Indice"; reconstrói DROP INDEX [idx] ON [schema].[tabela].
        var parts = c.Name.Split('.', 2);
        var table = parts.Length == 2 ? parts[0] : c.Name;
        var index = parts.Length == 2 ? parts[1] : c.Name;
        return $"DROP INDEX [{index}] ON [{c.Schema}].[{table}];";
    }
}
