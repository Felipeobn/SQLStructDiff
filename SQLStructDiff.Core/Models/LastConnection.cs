namespace SQLStructDiff.Core.Models;

/// <summary>
/// Última conexão usada em um painel (A ou B), para restaurar ao reabrir o app.
/// A senha fica em texto puro apenas em memória; o SettingsStore a criptografa
/// (DPAPI) ao persistir em disco.
/// </summary>
public sealed class LastConnection
{
    public string Server { get; set; } = string.Empty;
    public bool IntegratedSecurity { get; set; } = true;
    public string? UserId { get; set; }

    /// <summary>Senha em texto puro (apenas em memória).</summary>
    public string? Password { get; set; }

    public string? Database { get; set; }
}
