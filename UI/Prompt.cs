namespace SQLStructDiff.UI;

/// <summary>Diálogo simples de entrada de texto (substitui o InputBox do VB).</summary>
public sealed partial class Prompt : Form
{
    public Prompt()
    {
        InitializeComponent();
    }

    /// <summary>Exibe o diálogo e retorna o texto informado, ou null se cancelado/vazio.</summary>
    public static string? Show(IWin32Window owner, string title, string label, string initial = "")
    {
        using var form = new Prompt();
        form.Text = title;
        form._label.Text = label;
        form._input.Text = initial;

        return form.ShowDialog(owner) == DialogResult.OK && !string.IsNullOrWhiteSpace(form._input.Text)
            ? form._input.Text.Trim()
            : null;
    }
}
