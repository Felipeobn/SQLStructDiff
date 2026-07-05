namespace SQLStructDiff.Core.Models;

/// <summary>Schema completo de um banco: todos os objetos extraídos.</summary>
public sealed class DatabaseSchema
{
    public IReadOnlyList<DbObject> Objects { get; init; } = new List<DbObject>();
}
