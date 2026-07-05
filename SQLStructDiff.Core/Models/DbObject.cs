namespace SQLStructDiff.Core.Models;

/// <summary>
/// Representa um objeto de banco (tabela, view, procedure ou índice) com sua
/// definição DDL já normalizada (sem carimbo de data de geração).
/// </summary>
public sealed class DbObject
{
    public DbObjectType Type { get; init; }

    /// <summary>Schema do objeto (ex.: dbo).</summary>
    public string Schema { get; init; } = "dbo";

    /// <summary>Nome do objeto.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Nome totalmente qualificado usado como chave de comparação.</summary>
    public string FullName => $"[{Schema}].[{Name}]";

    /// <summary>DDL normalizado do objeto (para hash/diff).</summary>
    public string Definition { get; init; } = string.Empty;

    /// <summary>DDL original, preservado para geração de scripts executáveis.</summary>
    public string RawDefinition { get; init; } = string.Empty;

    /// <summary>Hash do conteúdo normalizado, usado para detectar alterações.</summary>
    public string Hash { get; init; } = string.Empty;

    /// <summary>
    /// Colunas estruturadas (apenas para Type == Table). Usado para gerar
    /// ALTER TABLE coluna-a-coluna em tabelas que existem nos dois bancos.
    /// </summary>
    public IReadOnlyList<TableColumn> Columns { get; init; } = Array.Empty<TableColumn>();

    /// <summary>Chave única considerando tipo + nome qualificado.</summary>
    public string Key => $"{Type}:{FullName}";
}
