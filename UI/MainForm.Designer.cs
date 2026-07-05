namespace SQLStructDiff.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _tips = new ToolTip(components);
        _toolbar = new FlowLayoutPanel();
        _refresh = new Button();
        _onlyDiff = new CheckBox();
        _paramsButton = new Button();
        _syncAToB = new Button();
        _syncBToA = new Button();
        _syncSelAToB = new Button();
        _syncSelBToA = new Button();
        _split = new SplitContainer();
        _tree = new TreeView();
        _grid = new DataGridView();
        _lnA = new DataGridViewTextBoxColumn();
        _txtA = new DataGridViewTextBoxColumn();
        _lnB = new DataGridViewTextBoxColumn();
        _txtB = new DataGridViewTextBoxColumn();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _busyOverlay = new Panel();
        _busyLabel = new Label();
        _busyBar = new ProgressBar();
        _toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_split).BeginInit();
        _split.Panel1.SuspendLayout();
        _split.Panel2.SuspendLayout();
        _split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
        _statusStrip.SuspendLayout();
        _busyOverlay.SuspendLayout();
        SuspendLayout();
        // 
        // _toolbar
        // 
        _toolbar.AutoSize = true;
        _toolbar.Controls.Add(_paramsButton);
        _toolbar.Controls.Add(_refresh);
        _toolbar.Controls.Add(_onlyDiff);
        _toolbar.Controls.Add(_syncAToB);
        _toolbar.Controls.Add(_syncBToA);
        _toolbar.Controls.Add(_syncSelAToB);
        _toolbar.Controls.Add(_syncSelBToA);
        _toolbar.Dock = DockStyle.Top;
        _toolbar.Location = new Point(0, 0);
        _toolbar.Name = "_toolbar";
        _toolbar.Padding = new Padding(8, 6, 8, 6);
        _toolbar.Size = new Size(1184, 43);
        _toolbar.TabIndex = 0;
        // 
        // _refresh
        // 
        _refresh.Font = new Font("Segoe MDL2 Assets", 12F);
        _refresh.Location = new Point(50, 8);
        _refresh.Name = "_refresh";
        _refresh.Size = new Size(36, 30);
        _refresh.TabIndex = 1;
        _refresh.Text = "";
        // 
        // _onlyDiff
        // 
        _onlyDiff.Anchor = AnchorStyles.None;
        _onlyDiff.AutoSize = true;
        _onlyDiff.Checked = true;
        _onlyDiff.CheckState = CheckState.Checked;
        _onlyDiff.Location = new Point(92, 9);
        _onlyDiff.Name = "_onlyDiff";
        _onlyDiff.Size = new Size(139, 19);
        _onlyDiff.TabIndex = 1;
        _onlyDiff.Text = "Mostrar só diferenças";
        // 
        // _paramsButton
        // 
        _paramsButton.Font = new Font("Segoe MDL2 Assets", 12F);
        _paramsButton.Location = new Point(8, 8);
        _paramsButton.Name = "_paramsButton";
        _paramsButton.Size = new Size(36, 30);
        _paramsButton.TabIndex = 0;
        _paramsButton.Text = "\uE713";
        // 
        // _syncAToB
        // 
        _syncAToB.Font = new Font("Segoe MDL2 Assets", 12F);
        _syncAToB.Location = new Point(235, 8);
        _syncAToB.Name = "_syncAToB";
        _syncAToB.Size = new Size(36, 30);
        _syncAToB.TabIndex = 3;
        _syncAToB.Text = "\uE896";
        // 
        // _syncBToA
        // 
        _syncBToA.Font = new Font("Segoe MDL2 Assets", 12F);
        _syncBToA.Location = new Point(277, 8);
        _syncBToA.Name = "_syncBToA";
        _syncBToA.Size = new Size(36, 30);
        _syncBToA.TabIndex = 4;
        _syncBToA.Text = "\uE898";
        //
        // _syncSelAToB
        //
        _syncSelAToB.Font = new Font("Segoe MDL2 Assets", 12F);
        _syncSelAToB.ForeColor = Color.SteelBlue;
        _syncSelAToB.Location = new Point(319, 8);
        _syncSelAToB.Name = "_syncSelAToB";
        _syncSelAToB.Size = new Size(36, 30);
        _syncSelAToB.TabIndex = 5;
        _syncSelAToB.Text = "\uE72A";
        //
        // _syncSelBToA
        //
        _syncSelBToA.Font = new Font("Segoe MDL2 Assets", 12F);
        _syncSelBToA.ForeColor = Color.SteelBlue;
        _syncSelBToA.Location = new Point(361, 8);
        _syncSelBToA.Name = "_syncSelBToA";
        _syncSelBToA.Size = new Size(36, 30);
        _syncSelBToA.TabIndex = 6;
        _syncSelBToA.Text = "\uE72B";
        // 
        // _split
        // 
        _split.Dock = DockStyle.Fill;
        _split.FixedPanel = FixedPanel.Panel1;
        _split.Location = new Point(0, 43);
        _split.Name = "_split";
        // 
        // _split.Panel1
        // 
        _split.Panel1.Controls.Add(_tree);
        // 
        // _split.Panel2
        // 
        _split.Panel2.Controls.Add(_grid);
        _split.Size = new Size(1184, 696);
        _split.SplitterDistance = 320;
        _split.TabIndex = 1;
        // 
        // _tree
        // 
        _tree.Dock = DockStyle.Fill;
        _tree.HideSelection = false;
        _tree.Location = new Point(0, 0);
        _tree.Name = "_tree";
        _tree.Size = new Size(320, 696);
        _tree.TabIndex = 0;
        // 
        // _grid
        // 
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _grid.Columns.AddRange(new DataGridViewColumn[] { _lnA, _txtA, _lnB, _txtB });
        _grid.Dock = DockStyle.Fill;
        _grid.Font = new Font("Consolas", 9F);
        _grid.Location = new Point(0, 0);
        _grid.Name = "_grid";
        _grid.ReadOnly = true;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _grid.Size = new Size(860, 696);
        _grid.TabIndex = 0;
        // 
        // _lnA
        // 
        _lnA.HeaderText = "#";
        _lnA.Name = "_lnA";
        _lnA.ReadOnly = true;
        _lnA.Width = 44;
        // 
        // _txtA
        // 
        _txtA.HeaderText = "Banco A";
        _txtA.Name = "_txtA";
        _txtA.ReadOnly = true;
        _txtA.Width = 560;
        // 
        // _lnB
        // 
        _lnB.HeaderText = "#";
        _lnB.Name = "_lnB";
        _lnB.ReadOnly = true;
        _lnB.Width = 44;
        // 
        // _txtB
        // 
        _txtB.HeaderText = "Banco B";
        _txtB.Name = "_txtB";
        _txtB.ReadOnly = true;
        _txtB.Width = 560;
        // 
        // _statusStrip
        // 
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel });
        _statusStrip.Location = new Point(0, 739);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(1184, 22);
        _statusStrip.TabIndex = 2;
        // 
        // _statusLabel
        // 
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(0, 17);
        // 
        // _busyOverlay
        // 
        _busyOverlay.BackColor = Color.White;
        _busyOverlay.BorderStyle = BorderStyle.FixedSingle;
        _busyOverlay.Controls.Add(_busyLabel);
        _busyOverlay.Controls.Add(_busyBar);
        _busyOverlay.Location = new Point(430, 330);
        _busyOverlay.Name = "_busyOverlay";
        _busyOverlay.Size = new Size(320, 96);
        _busyOverlay.TabIndex = 3;
        _busyOverlay.Visible = false;
        // 
        // _busyLabel
        // 
        _busyLabel.Dock = DockStyle.Fill;
        _busyLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _busyLabel.Location = new Point(0, 0);
        _busyLabel.Name = "_busyLabel";
        _busyLabel.Size = new Size(318, 74);
        _busyLabel.TabIndex = 0;
        _busyLabel.Text = "Comparando schemas...";
        _busyLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _busyBar
        // 
        _busyBar.Dock = DockStyle.Bottom;
        _busyBar.Location = new Point(0, 74);
        _busyBar.MarqueeAnimationSpeed = 30;
        _busyBar.Name = "_busyBar";
        _busyBar.Size = new Size(318, 20);
        _busyBar.Style = ProgressBarStyle.Marquee;
        _busyBar.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 761);
        Controls.Add(_split);
        Controls.Add(_toolbar);
        Controls.Add(_statusStrip);
        Controls.Add(_busyOverlay);
        Font = new Font("Segoe UI", 9F);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SQLStructDiff";
        WindowState = FormWindowState.Maximized;
        _toolbar.ResumeLayout(false);
        _toolbar.PerformLayout();
        _split.Panel1.ResumeLayout(false);
        _split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_split).EndInit();
        _split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _busyOverlay.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel _toolbar;
    private System.Windows.Forms.Button _refresh;
    private System.Windows.Forms.CheckBox _onlyDiff;
    private System.Windows.Forms.Button _paramsButton;
    private System.Windows.Forms.Button _syncAToB;
    private System.Windows.Forms.Button _syncBToA;
    private System.Windows.Forms.Button _syncSelAToB;
    private System.Windows.Forms.Button _syncSelBToA;
    private System.Windows.Forms.ToolTip _tips;
    private System.Windows.Forms.SplitContainer _split;
    private System.Windows.Forms.TreeView _tree;
    private System.Windows.Forms.DataGridView _grid;
    private System.Windows.Forms.DataGridViewTextBoxColumn _lnA;
    private System.Windows.Forms.DataGridViewTextBoxColumn _txtA;
    private System.Windows.Forms.DataGridViewTextBoxColumn _lnB;
    private System.Windows.Forms.DataGridViewTextBoxColumn _txtB;
    private System.Windows.Forms.StatusStrip _statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
    private System.Windows.Forms.Panel _busyOverlay;
    private System.Windows.Forms.Label _busyLabel;
    private System.Windows.Forms.ProgressBar _busyBar;
}
