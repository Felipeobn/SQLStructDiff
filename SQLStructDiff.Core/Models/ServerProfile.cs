namespace SQLStructDiff.Core.Models;

/// <summary>
/// Servidor salvo pelo usuário, para não precisar redigitar a cada abertura.
/// A senha é mantida em texto apenas em memória; o SettingsStore a criptografa
/// (DPAPI) ao persistir em disco.
/// </summary>
public sealed class ServerProfile
{
    /// <summary>Nome amigável do perfil (ex.: "Produção", "Homolog").</summary>
    public string Name { get; set; } = string.Empty;

    public string Server { get; set; } = string.Empty;
    public bool IntegratedSecurity { get; set; } = true;
    public string? UserId { get; set; }

    /// <summary>Senha em texto puro (apenas em memória).</summary>
    public string? Password { get; set; }

    /// <summary>Último database usado neste servidor (conveniência).</summary>
    public string? LastDatabase { get; set; }

    /// <summary>Cria um ConnectionInfo apontando para um database específico.</summary>
    public ConnectionInfo ToConnectionInfo(string database, string label) => new()
    {
        Label = label,
        Server = Server,
        Database = database,
        IntegratedSecurity = IntegratedSecurity,
        UserId = UserId,
        Password = Password
    };
}
