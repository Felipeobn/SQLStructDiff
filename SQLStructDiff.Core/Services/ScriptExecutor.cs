using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>
/// Executa um script SQL no banco alvo. Separa batches por GO e executa todos
/// dentro de uma única transação, fazendo rollback se qualquer batch falhar.
/// </summary>
public sealed partial class ScriptExecutor
{
    [GeneratedRegex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex GoSeparator();

    public async Task ExecuteAsync(ConnectionInfo target, string script, CancellationToken ct = default)
    {
        var batches = GoSeparator()
            .Split(script)
            .Select(b => b.Trim())
            .Where(b => b.Length > 0 && !IsOnlyComments(b))
            .ToList();

        await using var conn = new SqlConnection(target.BuildConnectionString());
        await conn.OpenAsync(ct);

        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(ct);
        try
        {
            foreach (var batch in batches)
            {
                // Ignora o controle de transação do script: a transação é gerida aqui.
                if (IsTransactionControl(batch)) continue;

                await using var cmd = new SqlCommand(batch, conn, tx);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private static bool IsOnlyComments(string batch) =>
        batch.Split('\n').All(l =>
        {
            var t = l.Trim();
            return t.Length == 0 || t.StartsWith("--");
        });

    private static bool IsTransactionControl(string batch)
    {
        var t = batch.Trim().TrimEnd(';').Trim().ToUpperInvariant();
        return t is "BEGIN TRANSACTION" or "COMMIT TRANSACTION" or "BEGIN TRAN" or "COMMIT TRAN"
            || t.StartsWith("SET XACT_ABORT");
    }
}
