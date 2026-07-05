namespace SQLStructDiff.Core.Models;

/// <summary>Comparação de um único objeto entre os bancos A e B.</summary>
public sealed class ObjectComparison
{
    public DbObjectType Type { get; init; }
    public string Schema { get; init; } = "dbo";
    public string Name { get; init; } = string.Empty;
    public string FullName => $"[{Schema}].[{Name}]";

    public CompareStatus Status { get; init; }

    /// <summary>Objeto no banco A (null quando só existe em B).</summary>
    public DbObject? ObjectA { get; init; }

    /// <summary>Objeto no banco B (null quando só existe em A).</summary>
    public DbObject? ObjectB { get; init; }
}
