namespace SQLStructDiff.UI;

partial class ScriptPreviewForm
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
        this._text = new System.Windows.Forms.TextBox();
        this._buttons = new System.Windows.Forms.FlowLayoutPanel();
        this._close = new System.Windows.Forms.Button();
        this._execute = new System.Windows.Forms.Button();
        this._save = new System.Windows.Forms.Button();
        this._copy = new System.Windows.Forms.Button();
        this._buttons.SuspendLayout();
        this.SuspendLayout();
        //
        // _text
        //
        this._text.Dock = System.Windows.Forms.DockStyle.Fill;
        this._text.Font = new System.Drawing.Font("Consolas", 9.5F);
        this._text.Location = new System.Drawing.Point(0, 0);
        this._text.Multiline = true;
        this._text.Name = "_text";
        this._text.ReadOnly = true;
        this._text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this._text.Size = new System.Drawing.Size(884, 559);
        this._text.TabIndex = 0;
        this._text.WordWrap = false;
        //
        // _buttons
        //
        this._buttons.AutoSize = true;
        this._buttons.Controls.Add(this._close);
        this._buttons.Controls.Add(this._execute);
        this._buttons.Controls.Add(this._save);
        this._buttons.Controls.Add(this._copy);
        this._buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
        this._buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        this._buttons.Location = new System.Drawing.Point(0, 559);
        this._buttons.Name = "_buttons";
        this._buttons.Padding = new System.Windows.Forms.Padding(8);
        this._buttons.Size = new System.Drawing.Size(884, 42);
        this._buttons.TabIndex = 1;
        //
        // _close
        //
        this._close.AutoSize = true;
        this._close.Name = "_close";
        this._close.Size = new System.Drawing.Size(60, 25);
        this._close.TabIndex = 0;
        this._close.Text = "Fechar";
        //
        // _execute
        //
        this._execute.AutoSize = true;
        this._execute.Name = "_execute";
        this._execute.Size = new System.Drawing.Size(110, 25);
        this._execute.TabIndex = 1;
        this._execute.Text = "Executar no alvo";
        //
        // _save
        //
        this._save.AutoSize = true;
        this._save.Name = "_save";
        this._save.Size = new System.Drawing.Size(80, 25);
        this._save.TabIndex = 2;
        this._save.Text = "Salvar .sql";
        //
        // _copy
        //
        this._copy.AutoSize = true;
        this._copy.Name = "_copy";
        this._copy.Size = new System.Drawing.Size(60, 25);
        this._copy.TabIndex = 3;
        this._copy.Text = "Copiar";
        //
        // ScriptPreviewForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(884, 601);
        this.Controls.Add(this._text);
        this.Controls.Add(this._buttons);
        this.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.MinimumSize = new System.Drawing.Size(800, 560);
        this.Name = "ScriptPreviewForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Script de sincronização";
        this._buttons.ResumeLayout(false);
        this._buttons.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TextBox _text;
    private System.Windows.Forms.FlowLayoutPanel _buttons;
    private System.Windows.Forms.Button _close;
    private System.Windows.Forms.Button _execute;
    private System.Windows.Forms.Button _save;
    private System.Windows.Forms.Button _copy;
}
