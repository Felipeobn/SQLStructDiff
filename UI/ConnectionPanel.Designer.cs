namespace SQLStructDiff.UI;

partial class ConnectionPanel
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
        _saveProfile = new Button();
        _deleteProfile = new Button();
        _box = new GroupBox();
        _layout = new TableLayoutPanel();
        _lblProfiles = new Label();
        _profiles = new ComboBox();
        _profileButtons = new FlowLayoutPanel();
        _lblServer = new Label();
        _server = new TextBox();
        _integrated = new CheckBox();
        _lblUser = new Label();
        _user = new TextBox();
        _lblPassword = new Label();
        _password = new TextBox();
        _connect = new Button();
        _lblDatabase = new Label();
        _databases = new ComboBox();
        _status = new Label();
        _box.SuspendLayout();
        _layout.SuspendLayout();
        _profileButtons.SuspendLayout();
        SuspendLayout();
        // 
        // _saveProfile
        // 
        _saveProfile.FlatAppearance.BorderSize = 0;
        _saveProfile.FlatStyle = FlatStyle.Flat;
        _saveProfile.Font = new Font("Segoe MDL2 Assets", 11F);
        _saveProfile.Location = new Point(2, 2);
        _saveProfile.Margin = new Padding(2);
        _saveProfile.Name = "_saveProfile";
        _saveProfile.Size = new Size(32, 28);
        _saveProfile.TabIndex = 0;
        _saveProfile.Text = "";
        _tips.SetToolTip(_saveProfile, "Salvar servidor");
        _saveProfile.UseVisualStyleBackColor = true;
        // 
        // _deleteProfile
        // 
        _deleteProfile.FlatAppearance.BorderSize = 0;
        _deleteProfile.FlatStyle = FlatStyle.Flat;
        _deleteProfile.Font = new Font("Segoe MDL2 Assets", 11F);
        _deleteProfile.ForeColor = Color.Firebrick;
        _deleteProfile.Location = new Point(38, 2);
        _deleteProfile.Margin = new Padding(2);
        _deleteProfile.Name = "_deleteProfile";
        _deleteProfile.Size = new Size(32, 28);
        _deleteProfile.TabIndex = 1;
        _deleteProfile.Text = "";
        _tips.SetToolTip(_deleteProfile, "Excluir servidor");
        _deleteProfile.UseVisualStyleBackColor = true;
        // 
        // _box
        // 
        _box.Controls.Add(_layout);
        _box.Dock = DockStyle.Fill;
        _box.Location = new Point(0, 0);
        _box.Name = "_box";
        _box.Padding = new Padding(12);
        _box.Size = new Size(456, 282);
        _box.TabIndex = 0;
        _box.TabStop = false;
        _box.Text = "Banco";
        // 
        // _layout
        // 
        _layout.ColumnCount = 3;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.ColumnStyles.Add(new ColumnStyle());
        _layout.Controls.Add(_lblProfiles, 0, 0);
        _layout.Controls.Add(_profiles, 1, 0);
        _layout.Controls.Add(_profileButtons, 2, 0);
        _layout.Controls.Add(_lblServer, 0, 1);
        _layout.Controls.Add(_server, 1, 1);
        _layout.Controls.Add(_integrated, 1, 2);
        _layout.Controls.Add(_lblUser, 0, 3);
        _layout.Controls.Add(_user, 1, 3);
        _layout.Controls.Add(_lblPassword, 0, 4);
        _layout.Controls.Add(_password, 1, 4);
        _layout.Controls.Add(_connect, 1, 5);
        _layout.Controls.Add(_lblDatabase, 0, 6);
        _layout.Controls.Add(_databases, 1, 6);
        _layout.Controls.Add(_status, 1, 7);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(12, 28);
        _layout.Name = "_layout";
        _layout.RowCount = 8;
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(432, 242);
        _layout.TabIndex = 0;
        // 
        // _lblProfiles
        // 
        _lblProfiles.Dock = DockStyle.Fill;
        _lblProfiles.Location = new Point(3, 0);
        _lblProfiles.Name = "_lblProfiles";
        _lblProfiles.Size = new Size(101, 32);
        _lblProfiles.TabIndex = 0;
        _lblProfiles.Text = "Escolher Servidor:";
        _lblProfiles.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _profiles
        // 
        _profiles.Dock = DockStyle.Fill;
        _profiles.DropDownStyle = ComboBoxStyle.DropDownList;
        _profiles.Location = new Point(110, 3);
        _profiles.Name = "_profiles";
        _profiles.Size = new Size(247, 23);
        _profiles.TabIndex = 1;
        // 
        // _profileButtons
        // 
        _profileButtons.AutoSize = true;
        _profileButtons.Controls.Add(_saveProfile);
        _profileButtons.Controls.Add(_deleteProfile);
        _profileButtons.Location = new Point(360, 0);
        _profileButtons.Margin = new Padding(0);
        _profileButtons.Name = "_profileButtons";
        _profileButtons.Size = new Size(72, 32);
        _profileButtons.TabIndex = 2;
        _profileButtons.WrapContents = false;
        // 
        // _lblServer
        // 
        _lblServer.Dock = DockStyle.Fill;
        _lblServer.Location = new Point(3, 32);
        _lblServer.Name = "_lblServer";
        _lblServer.Size = new Size(101, 29);
        _lblServer.TabIndex = 3;
        _lblServer.Text = "Servidor";
        _lblServer.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _server
        // 
        _server.Dock = DockStyle.Fill;
        _server.Location = new Point(110, 35);
        _server.Name = "_server";
        _server.Size = new Size(247, 23);
        _server.TabIndex = 4;
        // 
        // _integrated
        // 
        _integrated.AutoSize = true;
        _integrated.Checked = true;
        _integrated.CheckState = CheckState.Checked;
        _integrated.Location = new Point(110, 64);
        _integrated.Name = "_integrated";
        _integrated.Size = new Size(148, 19);
        _integrated.TabIndex = 5;
        _integrated.Text = "Autenticação Windows";
        // 
        // _lblUser
        // 
        _lblUser.Dock = DockStyle.Fill;
        _lblUser.Location = new Point(3, 86);
        _lblUser.Name = "_lblUser";
        _lblUser.Size = new Size(101, 29);
        _lblUser.TabIndex = 6;
        _lblUser.Text = "Usuário";
        _lblUser.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _user
        // 
        _user.Dock = DockStyle.Fill;
        _user.Enabled = false;
        _user.Location = new Point(110, 89);
        _user.Name = "_user";
        _user.Size = new Size(247, 23);
        _user.TabIndex = 7;
        // 
        // _lblPassword
        // 
        _lblPassword.Dock = DockStyle.Fill;
        _lblPassword.Location = new Point(3, 115);
        _lblPassword.Name = "_lblPassword";
        _lblPassword.Size = new Size(101, 29);
        _lblPassword.TabIndex = 8;
        _lblPassword.Text = "Senha";
        _lblPassword.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _password
        // 
        _password.Dock = DockStyle.Fill;
        _password.Enabled = false;
        _password.Location = new Point(110, 118);
        _password.Name = "_password";
        _password.Size = new Size(247, 23);
        _password.TabIndex = 9;
        _password.UseSystemPasswordChar = true;
        // 
        // _connect
        // 
        _connect.AutoSize = true;
        _connect.Location = new Point(110, 147);
        _connect.Name = "_connect";
        _connect.Size = new Size(75, 25);
        _connect.TabIndex = 10;
        _connect.Text = "Conectar";
        // 
        // _lblDatabase
        // 
        _lblDatabase.Dock = DockStyle.Fill;
        _lblDatabase.Location = new Point(3, 175);
        _lblDatabase.Name = "_lblDatabase";
        _lblDatabase.Size = new Size(101, 29);
        _lblDatabase.TabIndex = 11;
        _lblDatabase.Text = "Database";
        _lblDatabase.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _databases
        // 
        _databases.Dock = DockStyle.Fill;
        _databases.DropDownStyle = ComboBoxStyle.DropDownList;
        _databases.Enabled = false;
        _databases.Location = new Point(110, 178);
        _databases.Name = "_databases";
        _databases.Size = new Size(247, 23);
        _databases.TabIndex = 12;
        // 
        // _status
        // 
        _status.AutoSize = true;
        _status.ForeColor = Color.Gray;
        _status.Location = new Point(110, 204);
        _status.Name = "_status";
        _status.Size = new Size(0, 15);
        _status.TabIndex = 13;
        // 
        // ConnectionPanel
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_box);
        Name = "ConnectionPanel";
        Size = new Size(456, 282);
        _box.ResumeLayout(false);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _profileButtons.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.GroupBox _box;
    private System.Windows.Forms.TableLayoutPanel _layout;
    private System.Windows.Forms.Label _lblProfiles;
    private System.Windows.Forms.ComboBox _profiles;
    private System.Windows.Forms.FlowLayoutPanel _profileButtons;
    private System.Windows.Forms.Button _saveProfile;
    private System.Windows.Forms.Label _lblServer;
    private System.Windows.Forms.TextBox _server;
    private System.Windows.Forms.CheckBox _integrated;
    private System.Windows.Forms.Label _lblUser;
    private System.Windows.Forms.TextBox _user;
    private System.Windows.Forms.Label _lblPassword;
    private System.Windows.Forms.TextBox _password;
    private System.Windows.Forms.Button _connect;
    private System.Windows.Forms.Label _lblDatabase;
    private System.Windows.Forms.ComboBox _databases;
    private System.Windows.Forms.Label _status;
    private System.Windows.Forms.Button _deleteProfile;
    private System.Windows.Forms.ToolTip _tips;
}
