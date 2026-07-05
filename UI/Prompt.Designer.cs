namespace SQLStructDiff.UI;

partial class Prompt
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>Clean up any resources being used.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this._label = new System.Windows.Forms.Label();
        this._input = new System.Windows.Forms.TextBox();
        this._ok = new System.Windows.Forms.Button();
        this._cancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // _label
        //
        this._label.Location = new System.Drawing.Point(12, 12);
        this._label.Name = "_label";
        this._label.Size = new System.Drawing.Size(336, 20);
        this._label.TabIndex = 0;
        //
        // _input
        //
        this._input.Location = new System.Drawing.Point(12, 36);
        this._input.Name = "_input";
        this._input.Size = new System.Drawing.Size(336, 23);
        this._input.TabIndex = 1;
        //
        // _ok
        //
        this._ok.DialogResult = System.Windows.Forms.DialogResult.OK;
        this._ok.Location = new System.Drawing.Point(192, 76);
        this._ok.Name = "_ok";
        this._ok.Size = new System.Drawing.Size(75, 26);
        this._ok.TabIndex = 2;
        this._ok.Text = "OK";
        //
        // _cancel
        //
        this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._cancel.Location = new System.Drawing.Point(273, 76);
        this._cancel.Name = "_cancel";
        this._cancel.Size = new System.Drawing.Size(75, 26);
        this._cancel.TabIndex = 3;
        this._cancel.Text = "Cancelar";
        //
        // Prompt
        //
        this.AcceptButton = this._ok;
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this._cancel;
        this.ClientSize = new System.Drawing.Size(360, 120);
        this.Controls.Add(this._label);
        this.Controls.Add(this._input);
        this.Controls.Add(this._ok);
        this.Controls.Add(this._cancel);
        this.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "Prompt";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Prompt";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label _label;
    private System.Windows.Forms.TextBox _input;
    private System.Windows.Forms.Button _ok;
    private System.Windows.Forms.Button _cancel;
}
