using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>Compara dois schemas e classifica cada objeto.</summary>
public sealed class SchemaComparer
{
    public IReadOnlyList<ObjectComparison> Compare(
        DatabaseSchema schemaA,
        DatabaseSchema schemaB,
        ComparisonParameters? parameters = null)
    {
        parameters ??= new ComparisonParameters();

        var mapA = schemaA.Objects.ToDictionary(o => o.Key);
        var mapB = schemaB.Objects.ToDictionary(o => o.Key);

        var allKeys = mapA.Keys.Union(mapB.Keys);
        var result = new List<ObjectComparison>();

        foreach (var key in allKeys)
        {
            mapA.TryGetValue(key, out var a);
            mapB.TryGetValue(key, out var b);

            var status = (a, b) switch
            {
                (not null, null) => CompareStatus.OnlyInA,
                (null, not null) => CompareStatus.OnlyInB,
                _ => CompareBoth(a!, b!, parameters)
            };

            var reference = a ?? b!;
            result.Add(new ObjectComparison
            {
                Type = reference.Type,
                Schema = reference.Schema,
                Name = reference.Name,
                Status = status,
                ObjectA = a,
                ObjectB = b
            });
        }

        return result
            .OrderBy(r => r.Type)
            .ThenBy(r => r.Schema)
            .ThenBy(r => r.Name)
            .ToList();
    }

    private static CompareStatus CompareBoth(DbObject a, DbObject b, ComparisonParameters p)
    {
        if (a.Hash == b.Hash)
            return CompareStatus.Equal;

        // Quando a ordem dos campos não importa, tabelas que diferem apenas pela
        // ordem das colunas são consideradas iguais.
        if (!p.ConsiderColumnOrder && a.Type == DbObjectType.Table)
        {
            if (OrderInsensitiveSignature(a.Definition) == OrderInsensitiveSignature(b.Definition))
                return CompareStatus.Equal;
        }

        return CompareStatus.Different;
    }

    /// <summary>
    /// Assinatura de uma tabela insensível à ordem das colunas: ordena as linhas
    /// internas (definições de coluna e constraints) e mantém cabeçalho/rodapé.
    /// </summary>
    private static string OrderInsensitiveSignature(string tableDefinition)
    {
        var lines = tableDefinition.Replace("\r\n", "\n").Split('\n');
        if (lines.Length < 2) return tableDefinition;

        var header = lines[0].Trim();        // CREATE TABLE ... (
        var footer = lines[^1].Trim();       // );

        var inner = lines[1..^1]
            .Select(l => l.Trim().TrimEnd(','))
            .Where(l => l.Length > 0)
            .OrderBy(l => l, StringComparer.Ordinal)
            .ToList();

        return header + "\n" + string.Join("\n", inner) + "\n" + footer;
    }
}
