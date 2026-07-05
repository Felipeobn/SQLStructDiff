using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SQLStructDiff.Core.Models;

namespace SQLStructDiff.Core.Services;

/// <summary>
/// Carrega e salva as configurações em JSON em %APPDATA%\SQLStructDiff\settings.json.
/// Senhas são criptografadas com DPAPI (por usuário do Windows).
/// </summary>
public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _filePath;

    public SettingsStore(string? filePath = null)
    {
        _filePath = filePath ?? DefaultPath();
    }

    public string FilePath => _filePath;

    private static string DefaultPath()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SQLStructDiff");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "settings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_filePath))
            return new AppSettings();

        try
        {
            var json = File.ReadAllText(_filePath);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json, JsonOptions) ?? new SettingsDto();
            return FromDto(dto);
        }
        catch
        {
            // Arquivo corrompido: começa do zero em vez de quebrar a aplicação.
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        var dto = ToDto(settings);
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        File.WriteAllText(_filePath, json);
    }

    // ---- Mapeamento runtime <-> persistido (senha criptografada) ----

    private static SettingsDto ToDto(AppSettings s) => new()
    {
        Parameters = s.Parameters,
        Servers = s.Servers.Select(p => new ServerProfileDto
        {
            Name = p.Name,
            Server = p.Server,
            IntegratedSecurity = p.IntegratedSecurity,
            UserId = p.UserId,
            LastDatabase = p.LastDatabase,
            EncryptedPassword = Protect(p.Password)
        }).ToList(),
        LastConnectionA = ToConnDto(s.LastConnectionA),
        LastConnectionB = ToConnDto(s.LastConnectionB)
    };

    private static AppSettings FromDto(SettingsDto dto) => new()
    {
        Parameters = dto.Parameters ?? new ComparisonParameters(),
        Servers = (dto.Servers ?? new()).Select(d => new ServerProfile
        {
            Name = d.Name,
            Server = d.Server,
            IntegratedSecurity = d.IntegratedSecurity,
            UserId = d.UserId,
            LastDatabase = d.LastDatabase,
            Password = Unprotect(d.EncryptedPassword)
        }).ToList(),
        LastConnectionA = FromConnDto(dto.LastConnectionA),
        LastConnectionB = FromConnDto(dto.LastConnectionB)
    };

    private static LastConnectionDto? ToConnDto(LastConnection? c) => c is null ? null : new()
    {
        Server = c.Server,
        IntegratedSecurity = c.IntegratedSecurity,
        UserId = c.UserId,
        Database = c.Database,
        EncryptedPassword = Protect(c.Password)
    };

    private static LastConnection? FromConnDto(LastConnectionDto? d) => d is null ? null : new()
    {
        Server = d.Server,
        IntegratedSecurity = d.IntegratedSecurity,
        UserId = d.UserId,
        Database = d.Database,
        Password = Unprotect(d.EncryptedPassword)
    };

    private static string? Protect(string? plain)
    {
        if (string.IsNullOrEmpty(plain)) return null;
        var bytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(plain), null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(bytes);
    }

    private static string? Unprotect(string? encrypted)
    {
        if (string.IsNullOrEmpty(encrypted)) return null;
        try
        {
            var bytes = ProtectedData.Unprotect(
                Convert.FromBase64String(encrypted), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return null; // não foi possível descriptografar (ex.: outro usuário)
        }
    }

    // ---- DTOs de persistência ----

    private sealed class SettingsDto
    {
        public List<ServerProfileDto>? Servers { get; set; }
        public ComparisonParameters? Parameters { get; set; }
        public LastConnectionDto? LastConnectionA { get; set; }
        public LastConnectionDto? LastConnectionB { get; set; }
    }

    private sealed class LastConnectionDto
    {
        public string Server { get; set; } = string.Empty;
        public bool IntegratedSecurity { get; set; } = true;
        public string? UserId { get; set; }
        public string? Database { get; set; }
        public string? EncryptedPassword { get; set; }
    }

    private sealed class ServerProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public bool IntegratedSecurity { get; set; } = true;
        public string? UserId { get; set; }
        public string? LastDatabase { get; set; }
        public string? EncryptedPassword { get; set; }
    }
}
