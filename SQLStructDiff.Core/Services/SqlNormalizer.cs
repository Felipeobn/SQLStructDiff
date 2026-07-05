using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLStructDiff.Core.Services;

/// <summary>
/// Normaliza definições DDL para que a comparação ignore diferenças irrelevantes
/// (line endings, espaços em excesso, linhas em branco) e nunca dispare por
/// carimbos de data de geração.
/// </summary>
public static partial class SqlNormalizer
{
    [GeneratedRegex(@"[ \t]+")]
    private static partial Regex MultiSpace();

    [GeneratedRegex(@"[ \t]+\r?\n")]
    private static partial Regex TrailingSpace();

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex MultiBlankLines();

    /// <summary>
    /// Aplica normalização canônica: padroniza quebras de linha, remove espaços
    /// redundantes e linhas em branco excedentes, e faz trim geral.
    /// </summary>
    public static string Normalize(string? definition)
    {
        if (string.IsNullOrWhiteSpace(definition))
            return string.Empty;

        // Padroniza para \n para hashing/diff consistente entre origens.
        var text = definition.Replace("\r\n", "\n").Replace("\r", "\n");

        text = TrailingSpace().Replace(text, "\n");
        text = MultiSpace().Replace(text, " ");
        text = MultiBlankLines().Replace(text, "\n\n");

        return text.Trim();
    }

    /// <summary>Calcula um hash SHA-256 estável do conteúdo normalizado.</summary>
    public static string ComputeHash(string normalizedDefinition)
    {
        var bytes = Encoding.UTF8.GetBytes(normalizedDefinition);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
