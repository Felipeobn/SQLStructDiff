using Microsoft.Data.SqlClient;

namespace SQLStructDiff.Core.Models;

/// <summary>Dados de conexão de um banco. Mantidos apenas em memória (sessão).</summary>
public sealed class ConnectionInfo
{
    public string Server { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;

    /// <summary>Usa autenticação integrada do Windows quando true.</summary>
    public bool IntegratedSecurity { get; set; } = true;

    public string? UserId { get; set; }
    public string? Password { get; set; }

    /// <summary>Rótulo amigável para exibição (ex.: "A" ou "B").</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Nome curto para exibição: o database (ou o servidor, se sem database).</summary>
    public string DisplayName => string.IsNullOrWhiteSpace(Database) ? Server : Database;

    /// <summary>Nome completo para títulos: "database (servidor)".</summary>
    public string FullDisplayName =>
        string.IsNullOrWhiteSpace(Database) ? Server : $"{Database} ({Server})";

    public string BuildConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Server,
            TrustServerCertificate = true,
            ConnectTimeout = 15
        };

        if (!string.IsNullOrWhiteSpace(Database))
            builder.InitialCatalog = Database;

        if (IntegratedSecurity)
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            builder.UserID = UserId ?? string.Empty;
            builder.Password = Password ?? string.Empty;
        }

        return builder.ConnectionString;
    }
}
