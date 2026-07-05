namespace SQLStructDiff.Core.Models;

/// <summary>Metadados estruturados de uma coluna, usados para gerar ALTER TABLE.</summary>
public sealed class TableColumn
{
    public string Name { get; init; } = string.Empty;

    /// <summary>Tipo já formatado (ex.: "varchar(50)", "decimal(18,2)").</summary>
    public string TypeDefinition { get; init; } = string.Empty;

    public bool IsNullable { get; init; }
    public bool IsIdentity { get; init; }
    public string? IdentitySpec { get; init; }

    /// <summary>Definição do default (ex.: "((0))"), ou null.</summary>
    public string? DefaultDefinition { get; init; }

    /// <summary>Nome da constraint de default no banco, ou null.</summary>
    public string? DefaultConstraintName { get; init; }

    /// <summary>Posição da coluna na tabela.</summary>
    public int Ordinal { get; init; }

    /// <summary>
    /// Assinatura do tipo/nulabilidade — usada para detectar se um ALTER COLUMN
    /// é necessário (ignora default, que é tratado por constraint à parte).
    /// </summary>
    public string TypeSignature => $"{TypeDefinition.ToLowerInvariant()}|{(IsNullable ? "null" : "notnull")}";
}
