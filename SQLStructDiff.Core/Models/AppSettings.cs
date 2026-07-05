namespace SQLStructDiff.Core.Models;

/// <summary>Configurações persistidas da aplicação.</summary>
public sealed class AppSettings
{
    /// <summary>Servidores salvos pelo usuário.</summary>
    public List<ServerProfile> Servers { get; set; } = new();

    /// <summary>Parâmetros de comparação.</summary>
    public ComparisonParameters Parameters { get; set; } = new();

    /// <summary>Última conexão usada no painel A (banco da esquerda).</summary>
    public LastConnection? LastConnectionA { get; set; }

    /// <summary>Última conexão usada no painel B (banco da direita).</summary>
    public LastConnection? LastConnectionB { get; set; }
}
