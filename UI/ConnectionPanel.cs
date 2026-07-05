using SQLStructDiff.Core.Models;
using SQLStructDiff.Core.Services;

namespace SQLStructDiff.UI;

/// <summary>
/// Painel de conexão de um banco: escolhe/salva um servidor, conecta e seleciona
/// o database em um combo (a conexão é validada ao listar os databases).
/// As dependências são injetadas via <see cref="Initialize"/> (o construtor sem
/// parâmetros existe para o designer do Visual Studio).
/// </summary>
public sealed partial class ConnectionPanel : UserControl
{
    private string _label = string.Empty;
    private AppSettings _settings = new();
    private SettingsStore _store = new();
    private Action _onSettingsChanged = () => { };
    private readonly SchemaExtractor _extractor = new();

    /// <summary>Database a selecionar após conectar (restauração de sessão).</summary>
    private string? _preferredDatabase;

    public ConnectionPanel()
    {
        InitializeComponent();
    }

    /// <summary>Injeta as dependências e liga os eventos (chamado em runtime).</summary>
    public void Initialize(string label, AppSettings settings, SettingsStore store, Action onSettingsChanged)
    {
        _label = label;
        _settings = settings;
        _store = store;
        _onSettingsChanged = onSettingsChanged;

        WireEvents();
        RefreshProfiles();
        UpdateTitle();
    }

    /// <summary>Mostra o nome real do database selecionado no título do painel
    /// (em vez de "Banco A/B") assim que o usuário escolhe um database.</summary>
    private void UpdateTitle() =>
        _box.Text = _databases.SelectedItem is string db && !string.IsNullOrEmpty(db)
            ? db
            : $"Banco {_label}";

    private void WireEvents()
    {
        _integrated.CheckedChanged += (_, _) => _user.Enabled = _password.Enabled = !_integrated.Checked;
        _profiles.SelectedIndexChanged += (_, _) => ApplySelectedProfile();
        _databases.SelectedIndexChanged += (_, _) => UpdateTitle();
        _saveProfile.Click += (_, _) => SaveProfile();
        _deleteProfile.Click += (_, _) => DeleteProfile();
        _connect.Click += async (_, _) => await ConnectAsync();
    }

    // ---- Perfis salvos ----

    private void RefreshProfiles()
    {
        var current = (_profiles.SelectedItem as ServerProfile)?.Name;
        _profiles.Items.Clear();
        _profiles.Items.Add("<novo>");
        foreach (var p in _settings.Servers)
            _profiles.Items.Add(p);
        _profiles.DisplayMember = nameof(ServerProfile.Name);

        var idx = _settings.Servers.FindIndex(p => p.Name == current);
        _profiles.SelectedIndex = idx >= 0 ? idx + 1 : 0;
    }

    private void ApplySelectedProfile()
    {
        if (_profiles.SelectedItem is not ServerProfile p) return;
        _preferredDatabase = null; // ao trocar de perfil, usa o LastDatabase do próprio perfil
        _server.Text = p.Server;
        _integrated.Checked = p.IntegratedSecurity;
        _user.Text = p.UserId ?? string.Empty;
        _password.Text = p.Password ?? string.Empty;
        _databases.Items.Clear();
        _databases.Enabled = false;
        SetStatus($"Perfil '{p.Name}' carregado. Clique em Conectar.", Color.Gray);
    }

    private void SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(_server.Text))
        {
            SetStatus("Informe o servidor antes de salvar.", Color.Firebrick);
            return;
        }

        var existing = _profiles.SelectedItem as ServerProfile;
        var name = Prompt.Show(this, "Salvar servidor", "Nome do perfil:", existing?.Name ?? _server.Text.Trim());
        if (name is null) return;

        var profile = _settings.Servers.FirstOrDefault(p => p.Name == name) ?? new ServerProfile { Name = name };
        profile.Server = _server.Text.Trim();
        profile.IntegratedSecurity = _integrated.Checked;
        profile.UserId = _integrated.Checked ? null : _user.Text.Trim();
        profile.Password = _integrated.Checked ? null : _password.Text;
        if (_databases.SelectedItem is string db) profile.LastDatabase = db;

        if (!_settings.Servers.Contains(profile))
            _settings.Servers.Add(profile);

        _store.Save(_settings);
        _onSettingsChanged();
        RefreshProfiles();
        SetStatus($"Servidor '{name}' salvo.", Color.Green);
    }

    private void DeleteProfile()
    {
        if (_profiles.SelectedItem is not ServerProfile p) return;
        _settings.Servers.Remove(p);
        _store.Save(_settings);
        _onSettingsChanged();
        RefreshProfiles();
        SetStatus($"Servidor '{p.Name}' removido.", Color.Gray);
    }

    /// <summary>Atualiza a lista após mudança vinda do outro painel.</summary>
    public void NotifySettingsChanged() => RefreshProfiles();

    // ---- Conexão / databases ----

    private async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(_server.Text))
        {
            SetStatus("Informe o servidor.", Color.Firebrick);
            return;
        }

        _connect.Enabled = false;
        SetStatus("Conectando...", Color.Gray);
        try
        {
            var serverInfo = BuildServerInfo();
            var dbs = await _extractor.ListDatabasesAsync(serverInfo);

            _databases.Items.Clear();
            foreach (var db in dbs) _databases.Items.Add(db);
            _databases.Enabled = _databases.Items.Count > 0;

            var preferred = _preferredDatabase ?? (_profiles.SelectedItem as ServerProfile)?.LastDatabase;
            if (preferred is not null && _databases.Items.Contains(preferred))
                _databases.SelectedItem = preferred;
            else if (_databases.Items.Count > 0)
                _databases.SelectedIndex = 0;

            SetStatus($"Conectado. {dbs.Count} databases.", Color.Green);
        }
        catch (Exception ex)
        {
            SetStatus("Falha: " + ex.Message, Color.Firebrick);
        }
        finally
        {
            _connect.Enabled = true;
        }
    }

    private ConnectionInfo BuildServerInfo() => new()
    {
        Label = _label,
        Server = _server.Text.Trim(),
        IntegratedSecurity = _integrated.Checked,
        UserId = _user.Text.Trim(),
        Password = _password.Text
    };

    public ConnectionInfo GetConnectionInfo()
    {
        var info = BuildServerInfo();
        info.Database = _databases.SelectedItem as string ?? string.Empty;
        return info;
    }

    /// <summary>Snapshot da conexão atual, para persistir e restaurar ao reabrir o app.</summary>
    public LastConnection CaptureConnection() => new()
    {
        Server = _server.Text.Trim(),
        IntegratedSecurity = _integrated.Checked,
        UserId = _integrated.Checked ? null : _user.Text.Trim(),
        Password = _integrated.Checked ? null : _password.Text,
        Database = _databases.SelectedItem as string
    };

    /// <summary>Preenche os campos com a última conexão e reconecta para reexibir o database.</summary>
    public async Task RestoreAsync(LastConnection last)
    {
        if (string.IsNullOrWhiteSpace(last.Server)) return;

        // Se a última conexão corresponde a um perfil salvo, reseleciona-o no combo
        // (senão fica em "<novo>"); em ambos os casos preenchemos os campos abaixo.
        var match = _settings.Servers.FirstOrDefault(p =>
            string.Equals(p.Server, last.Server, StringComparison.OrdinalIgnoreCase) &&
            p.IntegratedSecurity == last.IntegratedSecurity &&
            string.Equals(p.UserId ?? string.Empty, last.UserId ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        _profiles.SelectedItem = (object?)match ?? "<novo>";

        // Sobrepõe o que o perfil colocou, garantindo os dados exatos da última sessão.
        _server.Text = last.Server;
        _integrated.Checked = last.IntegratedSecurity;
        _user.Text = last.UserId ?? string.Empty;
        _password.Text = last.Password ?? string.Empty;
        _preferredDatabase = last.Database;

        await ConnectAsync();
    }

    public bool Validate(out string error)
    {
        if (string.IsNullOrWhiteSpace(_server.Text))
        { error = $"Banco {_label}: informe o servidor."; return false; }
        if (_databases.SelectedItem is not string db || string.IsNullOrWhiteSpace(db))
        { error = $"Banco {_label}: conecte e selecione um database."; return false; }
        error = string.Empty;
        return true;
    }

    private void SetStatus(string message, Color color)
    {
        _status.ForeColor = color;
        _status.Text = message;
    }
}
