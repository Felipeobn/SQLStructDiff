namespace SQLStructDiff.UI;

partial class ConnectionForm
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
        _panels = new TableLayoutPanel();
        _panelA = new ConnectionPanel();
        _panelB = new ConnectionPanel();
        _buttons = new FlowLayoutPanel();
        _compare = new Button();
        _parameters = new Button();
        _statusBar = new Panel();
        _status = new Label();
        _panels.SuspendLayout();
        _buttons.SuspendLayout();
        _statusBar.SuspendLayout();
        SuspendLayout();
        // 
        // _panels
        // 
        _panels.ColumnCount = 2;
        _panels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _panels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _panels.Controls.Add(_panelA, 0, 0);
        _panels.Controls.Add(_panelB, 1, 0);
        _panels.Dock = DockStyle.Fill;
        _panels.Location = new Point(12, 12);
        _panels.Name = "_panels";
        _panels.RowCount = 1;
        _panels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _panels.Size = new Size(876, 331);
        _panels.TabIndex = 0;
        // 
        // _panelA
        // 
        _panelA.Dock = DockStyle.Fill;
        _panelA.Location = new Point(3, 3);
        _panelA.Name = "_panelA";
        _panelA.Size = new Size(432, 325);
        _panelA.TabIndex = 0;
        // 
        // _panelB
        // 
        _panelB.Dock = DockStyle.Fill;
        _panelB.Location = new Point(441, 3);
        _panelB.Name = "_panelB";
        _panelB.Size = new Size(432, 325);
        _panelB.TabIndex = 1;
        // 
        // _buttons
        // 
        _buttons.AutoSize = true;
        _buttons.Controls.Add(_compare);
        _buttons.Controls.Add(_parameters);
        _buttons.Dock = DockStyle.Bottom;
        _buttons.FlowDirection = FlowDirection.RightToLeft;
        _buttons.Location = new Point(12, 343);
        _buttons.Name = "_buttons";
        _buttons.Padding = new Padding(0, 8, 0, 0);
        _buttons.Size = new Size(876, 39);
        _buttons.TabIndex = 1;
        // 
        // _compare
        // 
        _compare.AutoSize = true;
        _compare.Location = new Point(790, 11);
        _compare.Name = "_compare";
        _compare.Size = new Size(83, 25);
        _compare.TabIndex = 0;
        _compare.Text = "Comparar →";
        // 
        // _parameters
        // 
        _parameters.AutoSize = true;
        _parameters.Location = new Point(698, 11);
        _parameters.Name = "_parameters";
        _parameters.Size = new Size(86, 25);
        _parameters.TabIndex = 1;
        _parameters.Text = "Parâmetros...";
        // 
        // _statusBar
        // 
        _statusBar.Controls.Add(_status);
        _statusBar.Dock = DockStyle.Bottom;
        _statusBar.Location = new Point(12, 382);
        _statusBar.Name = "_statusBar";
        _statusBar.Size = new Size(876, 26);
        _statusBar.TabIndex = 2;
        // 
        // _status
        // 
        _status.AutoSize = true;
        _status.Dock = DockStyle.Fill;
        _status.Location = new Point(0, 0);
        _status.Name = "_status";
        _status.Size = new Size(0, 15);
        _status.TabIndex = 0;
        // 
        // ConnectionForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 420);
        Controls.Add(_panels);
        Controls.Add(_buttons);
        Controls.Add(_statusBar);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(916, 459);
        Name = "ConnectionForm";
        Padding = new Padding(12);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SQLStructDiff — Conexão";
        _panels.ResumeLayout(false);
        _buttons.ResumeLayout(false);
        _buttons.PerformLayout();
        _statusBar.ResumeLayout(false);
        _statusBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _panels;
    private SQLStructDiff.UI.ConnectionPanel _panelB;
    private System.Windows.Forms.FlowLayoutPanel _buttons;
    private System.Windows.Forms.Button _compare;
    private System.Windows.Forms.Button _parameters;
    private System.Windows.Forms.Panel _statusBar;
    private System.Windows.Forms.Label _status;
    private ConnectionPanel _panelA;
}
