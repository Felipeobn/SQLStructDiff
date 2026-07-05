namespace SQLStructDiff.UI;

partial class ParametersForm
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
        this._considerColumnOrder = new System.Windows.Forms.CheckBox();
        this._hint = new System.Windows.Forms.Label();
        this._bidirectional = new System.Windows.Forms.CheckBox();
        this._bidiHint = new System.Windows.Forms.Label();
        this._ok = new System.Windows.Forms.Button();
        this._cancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // _considerColumnOrder
        //
        this._considerColumnOrder.AutoSize = true;
        this._considerColumnOrder.Location = new System.Drawing.Point(16, 16);
        this._considerColumnOrder.Name = "_considerColumnOrder";
        this._considerColumnOrder.Size = new System.Drawing.Size(196, 19);
        this._considerColumnOrder.TabIndex = 0;
        this._considerColumnOrder.Text = "Considerar ordem dos campos";
        //
        // _hint
        //
        this._hint.ForeColor = System.Drawing.Color.Gray;
        this._hint.Location = new System.Drawing.Point(34, 40);
        this._hint.Name = "_hint";
        this._hint.Size = new System.Drawing.Size(410, 40);
        this._hint.TabIndex = 1;
        this._hint.Text = "Quando DESMARCADO, tabelas cujas colunas existem em ambos os bancos, porém em ordem diferente, são consideradas IGUAIS.";
        //
        // _bidirectional
        //
        this._bidirectional.AutoSize = true;
        this._bidirectional.Location = new System.Drawing.Point(16, 92);
        this._bidirectional.Name = "_bidirectional";
        this._bidirectional.Size = new System.Drawing.Size(230, 19);
        this._bidirectional.TabIndex = 2;
        this._bidirectional.Text = "Permitir geração de script nos dois sentidos";
        //
        // _bidiHint
        //
        this._bidiHint.ForeColor = System.Drawing.Color.Gray;
        this._bidiHint.Location = new System.Drawing.Point(34, 116);
        this._bidiHint.Name = "_bidiHint";
        this._bidiHint.Size = new System.Drawing.Size(410, 40);
        this._bidiHint.TabIndex = 3;
        this._bidiHint.Text = "Quando DESMARCADO (unidirecional), só é possível gerar script do banco da esquerda para o da direita.";
        //
        // _ok
        //
        this._ok.DialogResult = System.Windows.Forms.DialogResult.OK;
        this._ok.Location = new System.Drawing.Point(285, 200);
        this._ok.Name = "_ok";
        this._ok.Size = new System.Drawing.Size(80, 26);
        this._ok.TabIndex = 2;
        this._ok.Text = "Salvar";
        //
        // _cancel
        //
        this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._cancel.Location = new System.Drawing.Point(370, 200);
        this._cancel.Name = "_cancel";
        this._cancel.Size = new System.Drawing.Size(80, 26);
        this._cancel.TabIndex = 3;
        this._cancel.Text = "Cancelar";
        //
        // ParametersForm
        //
        this.AcceptButton = this._ok;
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this._cancel;
        this.ClientSize = new System.Drawing.Size(460, 244);
        this.Controls.Add(this._considerColumnOrder);
        this.Controls.Add(this._hint);
        this.Controls.Add(this._bidirectional);
        this.Controls.Add(this._bidiHint);
        this.Controls.Add(this._ok);
        this.Controls.Add(this._cancel);
        this.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ParametersForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Parâmetros de comparação";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.CheckBox _considerColumnOrder;
    private System.Windows.Forms.Label _hint;
    private System.Windows.Forms.CheckBox _bidirectional;
    private System.Windows.Forms.Label _bidiHint;
    private System.Windows.Forms.Button _ok;
    private System.Windows.Forms.Button _cancel;
}
