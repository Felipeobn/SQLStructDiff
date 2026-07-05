using SQLStructDiff.Core.Models;
using SQLStructDiff.Core.Services;

namespace SQLStructDiff.UI;

/// <summary>Tela principal: comparação dos objetos e diff visual.</summary>
public sealed partial class MainForm : Form
{
    private readonly ConnectionInfo _connA = new();
    private readonly ConnectionInfo _connB = new();
    private readonly ComparisonParameters _parameters = new();
    private readonly Action _persistSettings = () => { };

    private readonly SchemaExtractor _extractor = new();
    private readonly SchemaComparer _comparer = new();
    private readonly DiffEngine _diff = new();

    private string _nameA = string.Empty;
    private string _nameB = string.Empty;

    private IReadOnlyList<ObjectComparison> _comparisons = new List<ObjectComparison>();

    // Seleção múltipla na árvore (Ctrl/Shift, estilo Windows Explorer).
    private readonly HashSet<TreeNode> _selected = new();
    private TreeNode? _anchor;
    private bool _mouseDown;
    private bool _syncingSelection;

    /// <summary>Construtor sem parâmetros — usado apenas pelo designer do Visual Studio.</summary>
    public MainForm()
    {
        InitializeComponent();
    }

    public MainForm(ConnectionInfo connA, ConnectionInfo connB,
        ComparisonParameters parameters, Action persistSettings) : this()
    {
        _connA = connA;
        _connB = connB;
        _parameters = parameters;
        _persistSettings = persistSettings;
        _nameA = connA.DisplayName;
        _nameB = connB.DisplayName;

        if (AppIcon.Value is { } icon) Icon = icon;

        Text = $"SQLStructDiff — {connA.FullDisplayName}  ×  {connB.FullDisplayName}";
        _txtA.HeaderText = _nameA;
        _txtB.HeaderText = _nameB;

        // Botões da toolbar são só ícones; o texto vai para o tooltip.
        _tips.SetToolTip(_paramsButton, "Parâmetros...");
        _tips.SetToolTip(_refresh, "Recarregar");
        _tips.SetToolTip(_syncAToB, $"Script {_nameA} → {_nameB} (tudo)");
        _tips.SetToolTip(_syncBToA, $"Script {_nameB} → {_nameA} (tudo)");
        _tips.SetToolTip(_syncSelAToB, $"Script {_nameA} → {_nameB} (selecionados)");
        _tips.SetToolTip(_syncSelBToA, $"Script {_nameB} → {_nameA} (selecionados)");

        ConfigureTreeMenu();
        ConfigureMultiSelect();

        _refresh.Click += async (_, _) => await LoadAsync();
        _onlyDiff.CheckedChanged += (_, _) => PopulateTree();
        _paramsButton.Click += async (_, _) => await OpenParametersAsync();
        _syncAToB.Click += (_, _) => GenerateSync(AllDiffs(), SyncDirection.AToB);
        _syncBToA.Click += (_, _) => GenerateSync(AllDiffs(), SyncDirection.BToA);
        _syncSelAToB.Click += (_, _) => GenerateForSelected(SyncDirection.AToB);
        _syncSelBToA.Click += (_, _) => GenerateForSelected(SyncDirection.BToA);

        ApplyDirectionMode();

        Resize += (_, _) => CenterBusyOverlay();
        Shown += async (_, _) => await LoadAsync();
    }

    /// <summary>No modo unidirecional só sobram os botões da esquerda para a direita (A→B).</summary>
    private void ApplyDirectionMode()
    {
        _syncBToA.Visible = _syncSelBToA.Visible = _parameters.Bidirectional;
    }

    private List<ObjectComparison> AllDiffs() =>
        _comparisons.Where(c => c.Status != CompareStatus.Equal).ToList();

    private async Task OpenParametersAsync()
    {
        using var form = new ParametersForm(_parameters);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _persistSettings();
            ApplyDirectionMode();
            await LoadAsync();
        }
    }

    private void ConfigureTreeMenu()
    {
        var menu = new ContextMenuStrip();
        var aToB = new ToolStripMenuItem($"Gerar script deste objeto: {_nameA} → {_nameB}");
        var bToA = new ToolStripMenuItem($"Gerar script deste objeto: {_nameB} → {_nameA}");
        aToB.Click += (_, _) => GenerateForSelected(SyncDirection.AToB);
        bToA.Click += (_, _) => GenerateForSelected(SyncDirection.BToA);
        menu.Items.Add(aToB);
        menu.Items.Add(bToA);

        // Habilita conforme houver objetos com diferença na seleção (única ou múltipla).
        menu.Opening += (_, e) =>
        {
            var count = SelectedDiffs().Count;
            aToB.Enabled = bToA.Enabled = count > 0;
            bToA.Available = _parameters.Bidirectional; // esconde B→A no modo unidirecional
            if (count == 0) e.Cancel = true;
        };
        _tree.ContextMenuStrip = menu;
        _tree.ShowNodeToolTips = true;
    }

    // ---- Seleção múltipla (Ctrl/Shift, estilo Windows Explorer) ----

    private void ConfigureMultiSelect()
    {
        _tree.DrawMode = TreeViewDrawMode.OwnerDrawText;
        _tree.DrawNode += TreeDrawNode;
        _tree.MouseDown += (_, _) => _mouseDown = true;
        _tree.MouseUp += (_, _) => _mouseDown = false;
        _tree.NodeMouseClick += TreeNodeClick;

        // Navegação por teclado (setas) = seleção única.
        _tree.AfterSelect += (_, e) =>
        {
            if (_mouseDown || _syncingSelection) return;
            SetSingleSelection(e.Node);
            ShowDiff(e.Node?.Tag as ObjectComparison);
        };
    }

    private void TreeNodeClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        var node = e.Node;
        if (node is null) return;

        // Alinha o nó focado sem disparar a lógica de teclado do AfterSelect.
        _syncingSelection = true;
        _tree.SelectedNode = node;
        _syncingSelection = false;

        if (node.Tag is not ObjectComparison)
        {
            SetSingleSelection(null);
            ShowDiff(null);
            return;
        }

        bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control;
        bool shift = (ModifierKeys & Keys.Shift) == Keys.Shift;

        if (e.Button == MouseButtons.Right)
        {
            // Como no Explorer: só reinicia a seleção se o nó não estiver nela.
            if (!_selected.Contains(node)) SetSingleSelection(node);
        }
        else if (ctrl)
        {
            if (!_selected.Add(node)) _selected.Remove(node);
            _anchor = node;
        }
        else if (shift && _anchor is not null)
        {
            SelectRange(_anchor, node);
        }
        else
        {
            SetSingleSelection(node);
        }

        _tree.Invalidate();
        ShowDiff(node.Tag as ObjectComparison);
    }

    private void SetSingleSelection(TreeNode? node)
    {
        _selected.Clear();
        if (node?.Tag is ObjectComparison)
        {
            _selected.Add(node);
            _anchor = node;
        }
        _tree.Invalidate();
    }

    private void SelectRange(TreeNode from, TreeNode to)
    {
        var leaves = LeafNodes();
        int i = leaves.IndexOf(from);
        int j = leaves.IndexOf(to);
        if (i < 0 || j < 0) { SetSingleSelection(to); return; }
        if (i > j) (i, j) = (j, i);

        _selected.Clear();
        for (int k = i; k <= j; k++) _selected.Add(leaves[k]);
        _anchor = from;
    }

    // Objetos (folhas) na ordem visível — filhos dos nós de tipo.
    private List<TreeNode> LeafNodes() =>
        _tree.Nodes.Cast<TreeNode>()
            .SelectMany(g => g.Nodes.Cast<TreeNode>())
            .ToList();

    private List<ObjectComparison> SelectedDiffs() =>
        _selected.Select(n => n.Tag).OfType<ObjectComparison>()
            .Where(c => c.Status != CompareStatus.Equal)
            .ToList();

    private void TreeDrawNode(object? sender, DrawTreeNodeEventArgs e)
    {
        if (e.Node is null) { e.DrawDefault = true; return; }

        bool selected = _selected.Contains(e.Node);
        Color back = selected ? SystemColors.Highlight : _tree.BackColor;
        Color fore = selected
            ? SystemColors.HighlightText
            : (e.Node.Tag is ObjectComparison c ? StatusColor(c.Status) : _tree.ForeColor);

        using var brush = new SolidBrush(back);
        e.Graphics.FillRectangle(brush, e.Bounds);
        TextRenderer.DrawText(e.Graphics, e.Node.Text, _tree.Font, e.Bounds, fore,
            TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
    }

    private void GenerateForSelected(SyncDirection direction)
    {
        var objs = SelectedDiffs();
        if (objs.Count == 0)
        {
            MessageBox.Show(this,
                "Selecione um ou mais objetos com diferença na árvore (use Ctrl/Shift para marcar vários).",
                "Somente selecionados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        GenerateSync(objs, direction);
    }

    private async Task LoadAsync()
    {
        SetBusy("Extraindo e comparando schemas...");
        try
        {
            var schemaA = await _extractor.ExtractAsync(_connA);
            var schemaB = await _extractor.ExtractAsync(_connB);
            _comparisons = _comparer.Compare(schemaA, schemaB, _parameters);
            PopulateTree();
            var diffCount = _comparisons.Count(c => c.Status != CompareStatus.Equal);
            SetIdle($"{_comparisons.Count} objetos · {diffCount} com diferença.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Erro ao comparar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetIdle("Falha ao comparar.");
        }
    }

    private void PopulateTree()
    {
        _tree.BeginUpdate();
        _tree.Nodes.Clear();
        _selected.Clear(); // os nós antigos serão descartados
        _anchor = null;

        foreach (DbObjectType type in Enum.GetValues<DbObjectType>())
        {
            var items = _comparisons.Where(c => c.Type == type);
            if (_onlyDiff.Checked)
                items = items.Where(c => c.Status != CompareStatus.Equal);

            var list = items.ToList();
            if (list.Count == 0) continue;

            var typeNode = new TreeNode($"{TypeLabel(type)} ({list.Count})") { Tag = null };
            foreach (var c in list)
            {
                var node = new TreeNode($"{StatusMark(c.Status)} {c.FullName}") { Tag = c };
                node.ForeColor = StatusColor(c.Status);
                node.ToolTipText = StatusDescription(c.Status);
                typeNode.Nodes.Add(node);
            }
            typeNode.Expand();
            _tree.Nodes.Add(typeNode);
        }
        _tree.EndUpdate();
        ShowDiff(null); // seleção foi limpa; zera o diff exibido
    }

    private void ShowDiff(ObjectComparison? c)
    {
        _grid.Rows.Clear();
        if (c is null) return;

        var rows = _diff.BuildSideBySide(c.ObjectA?.Definition, c.ObjectB?.Definition);
        foreach (var row in rows)
        {
            int idx = _grid.Rows.Add(
                row.Left.Number?.ToString() ?? "",
                row.Left.Text,
                row.Right.Number?.ToString() ?? "",
                row.Right.Text);

            ApplyCellColor(_grid.Rows[idx].Cells[_txtA.Index], row.Left.Kind);
            ApplyCellColor(_grid.Rows[idx].Cells[_txtB.Index], row.Right.Kind);
        }
    }

    private static void ApplyCellColor(DataGridViewCell cell, DiffLineKind kind)
    {
        cell.Style.BackColor = kind switch
        {
            DiffLineKind.Inserted => Color.FromArgb(204, 255, 204),
            DiffLineKind.Deleted => Color.FromArgb(255, 204, 204),
            DiffLineKind.Modified => Color.FromArgb(255, 245, 180),
            DiffLineKind.Imaginary => Color.FromArgb(240, 240, 240),
            _ => Color.White
        };
    }

    private void GenerateSync(List<ObjectComparison> toApply, SyncDirection direction)
    {
        if (toApply.Count == 0)
        {
            MessageBox.Show(this, "Não há diferenças para sincronizar.", "Sync",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var source = direction == SyncDirection.AToB ? _connA : _connB;
        var target = direction == SyncDirection.AToB ? _connB : _connA;
        var script = new SyncScriptGenerator().Generate(toApply, direction, source.DisplayName, target.DisplayName);
        using var preview = new ScriptPreviewForm(script, source, target);
        preview.ShowDialog(this);

        if (preview.Executed)
            _ = LoadAsync(); // re-comparar após aplicar
    }

    // ---- Indicador de execução ----

    private void SetBusy(string message)
    {
        _statusLabel.Text = message;
        _busyLabel.Text = message;
        SetControlsEnabled(false);
        CenterBusyOverlay();
        _busyOverlay.Visible = true;
        _busyOverlay.BringToFront();
        UseWaitCursor = true;
        // A barra marquee anima sozinha durante os await de LoadAsync (UI livre),
        // por isso não é mais preciso o Application.DoEvents().
    }

    private void SetIdle(string message)
    {
        _statusLabel.Text = message;
        _busyOverlay.Visible = false;
        SetControlsEnabled(true);
        UseWaitCursor = false;
    }

    private void SetControlsEnabled(bool enabled)
    {
        _refresh.Enabled = _paramsButton.Enabled = _onlyDiff.Enabled = enabled;
        _syncAToB.Enabled = _syncBToA.Enabled = enabled;
        _syncSelAToB.Enabled = _syncSelBToA.Enabled = enabled;
        _tree.Enabled = enabled;
    }

    private void CenterBusyOverlay() => _busyOverlay.Location = new Point(
        Math.Max(0, (ClientSize.Width - _busyOverlay.Width) / 2),
        Math.Max(0, (ClientSize.Height - _busyOverlay.Height) / 2));

    private static string TypeLabel(DbObjectType type) => type switch
    {
        DbObjectType.Table => "Tabelas",
        DbObjectType.View => "Views",
        DbObjectType.Procedure => "Procedures",
        DbObjectType.Index => "Índices",
        DbObjectType.Trigger => "Triggers",
        _ => type.ToString()
    };

    private static string StatusMark(CompareStatus s) => s switch
    {
        CompareStatus.Different => "≠",
        CompareStatus.OnlyInA => "◀",
        CompareStatus.OnlyInB => "▶",
        _ => "="
    };

    private string StatusDescription(CompareStatus s) => s switch
    {
        CompareStatus.Different => $"Existe nos dois, mas diferente entre {_nameA} e {_nameB}",
        CompareStatus.OnlyInA => $"Existe só em {_nameA}",
        CompareStatus.OnlyInB => $"Existe só em {_nameB}",
        _ => "Igual nos dois"
    };

    private static Color StatusColor(CompareStatus s) => s switch
    {
        CompareStatus.Different => Color.DarkGoldenrod,
        CompareStatus.OnlyInA => Color.SeaGreen,
        CompareStatus.OnlyInB => Color.SteelBlue,
        _ => Color.Black
    };
}
