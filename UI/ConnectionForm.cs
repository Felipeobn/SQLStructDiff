using System.ComponentModel;
using SQLStructDiff.Core.Models;
using SQLStructDiff.Core.Services;

namespace SQLStructDiff.UI;

/// <summary>Tela inicial: configura as conexões A e B e abre a comparação.</summary>
public sealed partial class ConnectionForm : Form
{
    private readonly SettingsStore _store = new();
    private AppSettings _settings = new();

    public ConnectionForm()
    {
        InitializeComponent();

        // Em tempo de design não carregamos settings do disco nem ligamos eventos.
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            return;

        _settings = _store.Load();
        _panelA.Initialize("A", _settings, _store, OnSettingsChanged);
        _panelB.Initialize("B", _settings, _store, OnSettingsChanged);

        _compare.Click += (_, _) => OpenCompare();
        _parameters.Click += (_, _) => OpenParameters();

        // Load dispara uma única vez (não repete ao voltar da comparação).
        Load += async (_, _) => await RestoreSessionAsync();
        FormClosing += (_, _) => SaveSession();
    }

    /// <summary>Restaura os dados de conexão usados na última sessão (reconectando).</summary>
    private async Task RestoreSessionAsync()
    {
        if (_settings.LastConnectionA is { } a) await _panelA.RestoreAsync(a);
        if (_settings.LastConnectionB is { } b) await _panelB.RestoreAsync(b);
    }

    /// <summary>Guarda a conexão atual de cada painel para restaurar no próximo início.</summary>
    private void SaveSession()
    {
        _settings.LastConnectionA = _panelA.CaptureConnection();
        _settings.LastConnectionB = _panelB.CaptureConnection();
        _store.Save(_settings);
    }

    private void OnSettingsChanged()
    {
        _panelA.NotifySettingsChanged();
        _panelB.NotifySettingsChanged();
    }

    private void OpenParameters()
    {
        using var form = new ParametersForm(_settings.Parameters);
        if (form.ShowDialog(this) == DialogResult.OK)
            _store.Save(_settings);
    }

    private void OpenCompare()
    {
        if (!_panelA.Validate(out var errA)) { ShowError(errA); return; }
        if (!_panelB.Validate(out var errB)) { ShowError(errB); return; }

        SaveSession(); // guarda a conexão usada para restaurar no próximo início

        var main = new MainForm(_panelA.GetConnectionInfo(), _panelB.GetConnectionInfo(),
            _settings.Parameters, () => _store.Save(_settings));
        Hide();
        main.FormClosed += (_, _) => Show();
        main.Show();
    }

    private void ShowError(string message)
    {
        _status.ForeColor = Color.Firebrick;
        _status.Text = message;
    }
}
