using SQLStructDiff.Core.Models;
using SQLStructDiff.Core.Services;

namespace SQLStructDiff.UI;

/// <summary>Preview do script de sync, com opções de copiar, salvar e executar.</summary>
public sealed partial class ScriptPreviewForm : Form
{
    private readonly string _script = string.Empty;
    private readonly ConnectionInfo _source = new();
    private readonly ConnectionInfo _target = new();

    /// <summary>True se o script foi executado com sucesso.</summary>
    public bool Executed { get; private set; }

    /// <summary>Construtor sem parâmetros — usado apenas pelo designer do Visual Studio.</summary>
    public ScriptPreviewForm()
    {
        InitializeComponent();
    }

    public ScriptPreviewForm(string script, ConnectionInfo source, ConnectionInfo target) : this()
    {
        _script = script;
        _source = source;
        _target = target;

        Text = $"Script de sincronização: {source.DisplayName} → {target.DisplayName} (alvo: {target.FullDisplayName})";
        _text.Text = script;

        _copy.Click += (_, _) => Clipboard.SetText(_script);
        _save.Click += (_, _) => Save();
        _execute.Click += async (_, _) => await ExecuteAsync();
        _close.Click += (_, _) => Close();
    }

    private void Save()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Script SQL (*.sql)|*.sql|Todos (*.*)|*.*",
            FileName = $"sync_{SafeName(_source.DisplayName)}_para_{SafeName(_target.DisplayName)}.sql"
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            File.WriteAllText(dialog.FileName, _script);
    }

    /// <summary>Remove caracteres inválidos para nome de arquivo (ex.: "\" de servidor\instância).</summary>
    private static string SafeName(string name) =>
        string.Concat(name.Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));

    private async Task ExecuteAsync()
    {
        var confirm = MessageBox.Show(this,
            $"Executar o script no banco ALVO '{_target.Database}' em {_target.Server}?\n\n" +
            "A execução é feita em uma transação; em caso de erro, é revertida (rollback).",
            "Confirmar execução", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        _execute.Enabled = false;
        UseWaitCursor = true;
        try
        {
            await new ScriptExecutor().ExecuteAsync(_target, _script);
            Executed = true;
            MessageBox.Show(this, "Script executado com sucesso.", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Erro ao executar (rollback aplicado):\n\n" + ex.Message,
                "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _execute.Enabled = true;
            UseWaitCursor = false;
        }
    }
}
