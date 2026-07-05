using SQLStructDiff.Core.Models;

namespace SQLStructDiff.UI;

/// <summary>Tela de parâmetros da comparação. Edita a instância recebida ao confirmar.</summary>
public sealed partial class ParametersForm : Form
{
    private readonly ComparisonParameters? _parameters;

    /// <summary>Construtor sem parâmetros — usado apenas pelo designer do Visual Studio.</summary>
    public ParametersForm()
    {
        InitializeComponent();
    }

    public ParametersForm(ComparisonParameters parameters) : this()
    {
        _parameters = parameters;
        _considerColumnOrder.Checked = parameters.ConsiderColumnOrder;
        _bidirectional.Checked = parameters.Bidirectional;
        _ok.Click += (_, _) => Apply();
    }

    private void Apply()
    {
        if (_parameters is null) return;
        _parameters.ConsiderColumnOrder = _considerColumnOrder.Checked;
        _parameters.Bidirectional = _bidirectional.Checked;
    }
}
