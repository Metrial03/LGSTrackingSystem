namespace LGSTrackingSystem
{
    partial class LoginPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPage));
            labelTitle = new Label();
            label1 = new Label();
            textBoxUsername = new TextBox();
            label2 = new Label();
            textBoxPassword = new TextBox();
            checkBoxShowPassword = new CheckBox();
            btnLogin = new Button();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            labelTitle.ForeColor = Color.DarkSlateBlue;
            labelTitle.Location = new Point(120, 66);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(279, 37);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "LGS Tracking System";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            label1.Location = new Point(120, 140);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 1;
            label1.Text = "Username:";
            // 
            // textBoxUsername
            // 
            textBoxUsername.Font = new Font("Segoe UI", 10F);
            textBoxUsername.Location = new Point(220, 138);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(170, 25);
            textBoxUsername.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            label2.Location = new Point(120, 185);
            label2.Name = "label2";
            label2.Size = new Size(80, 20);
            label2.TabIndex = 3;
            label2.Text = "Password:";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Font = new Font("Segoe UI", 10F);
            textBoxPassword.Location = new Point(220, 183);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(170, 25);
            textBoxPassword.TabIndex = 4;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // checkBoxShowPassword
            // 
            checkBoxShowPassword.AutoSize = true;
            checkBoxShowPassword.Font = new Font("Segoe UI", 9F);
            checkBoxShowPassword.Location = new Point(220, 215);
            checkBoxShowPassword.Name = "checkBoxShowPassword";
            checkBoxShowPassword.Size = new Size(108, 19);
            checkBoxShowPassword.TabIndex = 5;
            checkBoxShowPassword.Text = "Show Password";
            checkBoxShowPassword.UseVisualStyleBackColor = true;
            checkBoxShowPassword.CheckedChanged += checkBoxShowPassword_CheckedChanged;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.MediumSlateBlue;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(220, 250);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(170, 35);
            btnLogin.TabIndex = 6;
            btnLogin.Text = "LOGIN";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Lavender;
            ClientSize = new Size(550, 350);
            Controls.Add(labelTitle);
            Controls.Add(label1);
            Controls.Add(textBoxUsername);
            Controls.Add(label2);
            Controls.Add(textBoxPassword);
            Controls.Add(checkBoxShowPassword);
            Controls.Add(btnLogin);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginPage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LGS Tracking System - Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelTitle;
        private Label label1;
        private TextBox textBoxUsername;
        private Label label2;
        private TextBox textBoxPassword;
        private CheckBox checkBoxShowPassword;
        private Button btnLogin;
    }
}
