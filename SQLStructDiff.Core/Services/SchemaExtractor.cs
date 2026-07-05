using System.Text;
using Microsoft.Data.SqlClient;
using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>
/// Extrai a estrutura de um banco SQL Server. Views e Procedures vêm de
/// sys.sql_modules (sem carimbo de data). Tabelas e Índices são reconstruídos
/// do catálogo do sistema de forma determinística.
/// </summary>
public sealed class SchemaExtractor
{
    public async Task<bool> TestConnectionAsync(ConnectionInfo info, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(info.BuildConnectionString());
        await conn.OpenAsync(ct);
        return true;
    }

    /// <summary>
    /// Conecta ao servidor e lista os databases disponíveis (online, não-sistema).
    /// Valida a conexão como efeito colateral — se abrir, a conexão está OK.
    /// </summary>
    public async Task<IReadOnlyList<string>> ListDatabasesAsync(ConnectionInfo serverInfo, CancellationToken ct = default)
    {
        // Conecta sem catálogo fixo (usa o default do login).
        var info = new ConnectionInfo
        {
            Server = serverInfo.Server,
            Database = string.Empty,
            IntegratedSecurity = serverInfo.IntegratedSecurity,
            UserId = serverInfo.UserId,
            Password = serverInfo.Password
        };

        await using var conn = new SqlConnection(info.BuildConnectionString());
        await conn.OpenAsync(ct);

        const string sql = @"
SELECT name FROM sys.databases
WHERE database_id > 4 AND state = 0
ORDER BY name;";

        var result = new List<string>();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            result.Add(reader.GetString(0));
        return result;
    }

    public async Task<DatabaseSchema> ExtractAsync(ConnectionInfo info, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(info.BuildConnectionString());
        await conn.OpenAsync(ct);

        var objects = new List<DbObject>();
        objects.AddRange(await ExtractModulesAsync(conn, DbObjectType.View, "V", ct));
        objects.AddRange(await ExtractModulesAsync(conn, DbObjectType.Procedure, "P", ct));
        objects.AddRange(await ExtractTablesAsync(conn, ct));
        objects.AddRange(await ExtractIndexesAsync(conn, ct));

        return new DatabaseSchema { Objects = objects };
    }

    private static DbObject Build(DbObjectType type, string schema, string name, string rawDefinition,
        IReadOnlyList<TableColumn>? columns = null)
    {
        var normalized = SqlNormalizer.Normalize(rawDefinition);
        return new DbObject
        {
            Type = type,
            Schema = schema,
            Name = name,
            Definition = normalized,
            RawDefinition = rawDefinition?.Trim() ?? string.Empty,
            Hash = SqlNormalizer.ComputeHash(normalized),
            Columns = columns ?? Array.Empty<TableColumn>()
        };
    }

    // ---- Views e Procedures (texto original, sem data) ----

    private static async Task<List<DbObject>> ExtractModulesAsync(
        SqlConnection conn, DbObjectType type, string objType, CancellationToken ct)
    {
        const string sql = @"
SELECT s.name AS SchemaName, o.name AS ObjectName, m.definition AS Definition
FROM sys.sql_modules m
JOIN sys.objects o ON o.object_id = m.object_id
JOIN sys.schemas s ON s.schema_id = o.schema_id
WHERE o.type = @type AND o.is_ms_shipped = 0
ORDER BY s.name, o.name;";

        var result = new List<DbObject>();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@type", objType);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Add(Build(type,
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? string.Empty : reader.GetString(2)));
        }
        return result;
    }

    // ---- Tabelas: CREATE TABLE reconstruído ----

    private static async Task<List<DbObject>> ExtractTablesAsync(SqlConnection conn, CancellationToken ct)
    {
        const string columnsSql = @"
SELECT s.name AS SchemaName, t.name AS TableName, c.name AS ColumnName,
       ty.name AS TypeName, c.max_length, c.precision, c.scale,
       c.is_nullable, c.is_identity,
       ic.seed_value, ic.increment_value,
       dc.definition AS DefaultDefinition, dc.name AS DefaultName, c.column_id
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.columns c ON c.object_id = t.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
LEFT JOIN sys.identity_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
LEFT JOIN sys.default_constraints dc ON dc.object_id = c.default_object_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name, c.column_id;";

        // (schema, table) -> colunas estruturadas
        var tables = new Dictionary<(string, string), List<TableColumn>>();

        await using (var cmd = new SqlCommand(columnsSql, conn))
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var schema = reader.GetString(0);
                var table = reader.GetString(1);
                var col = reader.GetString(2);
                var typeName = reader.GetString(3);
                var maxLen = reader.GetInt16(4);
                var precision = reader.GetByte(5);
                var scale = reader.GetByte(6);
                var nullable = reader.GetBoolean(7);
                var isIdentity = reader.GetBoolean(8);
                var seed = reader.IsDBNull(9) ? (object?)null : reader.GetValue(9);
                var incr = reader.IsDBNull(10) ? (object?)null : reader.GetValue(10);
                var defDef = reader.IsDBNull(11) ? null : reader.GetString(11);
                var defName = reader.IsDBNull(12) ? null : reader.GetString(12);
                var columnId = reader.GetInt32(13);

                var column = new TableColumn
                {
                    Name = col,
                    TypeDefinition = FormatType(typeName, maxLen, precision, scale),
                    IsNullable = nullable,
                    IsIdentity = isIdentity,
                    IdentitySpec = isIdentity ? $"IDENTITY({seed ?? 1},{incr ?? 1})" : null,
                    DefaultDefinition = defDef,
                    DefaultConstraintName = defName,
                    Ordinal = columnId
                };

                var key = (schema, table);
                if (!tables.TryGetValue(key, out var list))
                    tables[key] = list = new List<TableColumn>();
                list.Add(column);
            }
        }

        var pks = await ExtractPrimaryKeysAsync(conn, ct);

        var result = new List<DbObject>();
        foreach (var ((schema, table), columns) in tables)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE [").Append(schema).Append("].[").Append(table).AppendLine("] (");
            sb.Append(string.Join(",\n", columns.OrderBy(c => c.Ordinal).Select(c => ColumnDdl(c))));

            if (pks.TryGetValue((schema, table), out var pkCols))
            {
                sb.Append(",\n    PRIMARY KEY (")
                  .Append(string.Join(", ", pkCols.Select(c => $"[{c}]")))
                  .Append(')');
            }

            sb.AppendLine().Append(");");
            result.Add(Build(DbObjectType.Table, schema, table, sb.ToString(), columns));
        }
        return result;
    }

    /// <summary>Monta a linha DDL de uma coluna (usada no CREATE TABLE e no ADD/ALTER).</summary>
    internal static string ColumnDdl(TableColumn c, bool indent = true)
    {
        var sb = new StringBuilder();
        if (indent) sb.Append("    ");
        sb.Append('[').Append(c.Name).Append("] ").Append(c.TypeDefinition);
        if (c.IsIdentity && c.IdentitySpec is not null)
            sb.Append(' ').Append(c.IdentitySpec);
        sb.Append(c.IsNullable ? " NULL" : " NOT NULL");
        if (c.DefaultDefinition is not null)
            sb.Append(" DEFAULT ").Append(c.DefaultDefinition);
        return sb.ToString();
    }

    private static async Task<Dictionary<(string, string), List<string>>> ExtractPrimaryKeysAsync(
        SqlConnection conn, CancellationToken ct)
    {
        const string sql = @"
SELECT s.name AS SchemaName, t.name AS TableName, c.name AS ColumnName
FROM sys.key_constraints kc
JOIN sys.tables t ON t.object_id = kc.parent_object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.index_columns ix ON ix.object_id = kc.parent_object_id AND ix.index_id = kc.unique_index_id
JOIN sys.columns c ON c.object_id = ix.object_id AND c.column_id = ix.column_id
WHERE kc.type = 'PK'
ORDER BY s.name, t.name, ix.key_ordinal;";

        var map = new Dictionary<(string, string), List<string>>();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var key = (reader.GetString(0), reader.GetString(1));
            if (!map.TryGetValue(key, out var list))
                map[key] = list = new List<string>();
            list.Add(reader.GetString(2));
        }
        return map;
    }

    // ---- Índices (exceto PK) ----

    private static async Task<List<DbObject>> ExtractIndexesAsync(SqlConnection conn, CancellationToken ct)
    {
        const string sql = @"
SELECT s.name AS SchemaName, t.name AS TableName, i.name AS IndexName,
       i.is_unique, i.type_desc, c.name AS ColumnName, ic.is_descending_key,
       ic.is_included_column, ic.key_ordinal
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE i.is_primary_key = 0 AND i.is_unique_constraint = 0
  AND i.type IN (1, 2) AND t.is_ms_shipped = 0 AND i.name IS NOT NULL
ORDER BY s.name, t.name, i.name, ic.is_included_column, ic.key_ordinal;";

        // chave do índice -> dados acumulados
        var indexes = new Dictionary<string, IndexBuilder>();

        await using (var cmd = new SqlCommand(sql, conn))
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var schema = reader.GetString(0);
                var table = reader.GetString(1);
                var index = reader.GetString(2);
                var isUnique = reader.GetBoolean(3);
                var typeDesc = reader.GetString(4);
                var column = reader.GetString(5);
                var descending = reader.GetBoolean(6);
                var included = reader.GetBoolean(7);

                var key = $"{schema}.{table}.{index}";
                if (!indexes.TryGetValue(key, out var b))
                {
                    indexes[key] = b = new IndexBuilder
                    {
                        Schema = schema, Table = table, Name = index,
                        IsUnique = isUnique, TypeDesc = typeDesc
                    };
                }

                if (included)
                    b.IncludedColumns.Add(column);
                else
                    b.KeyColumns.Add($"[{column}]{(descending ? " DESC" : " ASC")}");
            }
        }

        var result = new List<DbObject>();
        foreach (var b in indexes.Values)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE ");
            if (b.IsUnique) sb.Append("UNIQUE ");
            sb.Append(b.TypeDesc == "CLUSTERED" ? "CLUSTERED " : "NONCLUSTERED ");
            sb.Append("INDEX [").Append(b.Name).Append("] ON [")
              .Append(b.Schema).Append("].[").Append(b.Table).Append("] (")
              .Append(string.Join(", ", b.KeyColumns)).Append(')');

            if (b.IncludedColumns.Count > 0)
                sb.Append(" INCLUDE (")
                  .Append(string.Join(", ", b.IncludedColumns.Select(c => $"[{c}]")))
                  .Append(')');

            sb.Append(';');
            // Índice identificado por tabela+nome para casar entre bancos.
            result.Add(Build(DbObjectType.Index, b.Schema, $"{b.Table}.{b.Name}", sb.ToString()));
        }
        return result;
    }

    private sealed class IndexBuilder
    {
        public string Schema = "";
        public string Table = "";
        public string Name = "";
        public bool IsUnique;
        public string TypeDesc = "";
        public List<string> KeyColumns = new();
        public List<string> IncludedColumns = new();
    }

    // ---- Formatação de tipos ----

    private static string FormatType(string typeName, short maxLength, byte precision, byte scale)
    {
        var t = typeName.ToLowerInvariant();
        return t switch
        {
            "varchar" or "char" or "varbinary" or "binary" =>
                $"{typeName}({(maxLength == -1 ? "max" : maxLength.ToString())})",
            "nvarchar" or "nchar" =>
                $"{typeName}({(maxLength == -1 ? "max" : (maxLength / 2).ToString())})",
            "decimal" or "numeric" =>
                $"{typeName}({precision},{scale})",
            "datetime2" or "time" or "datetimeoffset" =>
                $"{typeName}({scale})",
            _ => typeName
        };
    }
}
