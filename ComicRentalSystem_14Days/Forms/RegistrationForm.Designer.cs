namespace ComicRentalSystem_14Days.Forms
{
    partial class RegistrationForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.GroupBox gbAccountCredentials;
        private System.Windows.Forms.GroupBox gbMemberInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.gbAccountCredentials = new System.Windows.Forms.GroupBox();
            this.gbMemberInfo = new System.Windows.Forms.GroupBox();

            this.gbAccountCredentials.SuspendLayout();
            this.gbMemberInfo.SuspendLayout();
            this.SuspendLayout();
            //
            // gbAccountCredentials
            //
            this.gbAccountCredentials.Controls.Add(this.lblUsername);
            this.gbAccountCredentials.Controls.Add(this.txtUsername);
            this.gbAccountCredentials.Controls.Add(this.lblPassword);
            this.gbAccountCredentials.Controls.Add(this.txtPassword);
            this.gbAccountCredentials.Controls.Add(this.lblConfirmPassword);
            this.gbAccountCredentials.Controls.Add(this.txtConfirmPassword);
            this.gbAccountCredentials.Location = new System.Drawing.Point(15, 15);
            this.gbAccountCredentials.Name = "gbAccountCredentials";
            this.gbAccountCredentials.Padding = new System.Windows.Forms.Padding(10);
            this.gbAccountCredentials.Size = new System.Drawing.Size(420, 130); // Adjusted size
            this.gbAccountCredentials.TabIndex = 0;
            this.gbAccountCredentials.TabStop = false;
            this.gbAccountCredentials.Text = "Account Credentials";
            //
            // lblUsername
            //
            this.lblUsername.Location = new System.Drawing.Point(15, 30);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(125, 23); // Adjusted width
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username:";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtUsername
            //
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtUsername.Location = new System.Drawing.Point(150, 30);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(250, 23);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Validating += new System.ComponentModel.CancelEventHandler(this.txtUsername_Validating);
            //
            // lblPassword
            //
            this.lblPassword.Location = new System.Drawing.Point(15, 60);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(125, 23);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtPassword
            //
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtPassword.Location = new System.Drawing.Point(150, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(250, 23);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassword_Validating);
            //
            // lblConfirmPassword
            //
            this.lblConfirmPassword.Location = new System.Drawing.Point(15, 90);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(125, 23);
            this.lblConfirmPassword.TabIndex = 4;
            this.lblConfirmPassword.Text = "Confirm Password:";
            this.lblConfirmPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtConfirmPassword
            //
            this.txtConfirmPassword.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtConfirmPassword.Location = new System.Drawing.Point(150, 90);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(250, 23);
            this.txtConfirmPassword.TabIndex = 5;
            this.txtConfirmPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtConfirmPassword_Validating);
            //
            // gbMemberInfo
            //
            this.gbMemberInfo.Controls.Add(this.lblName);
            this.gbMemberInfo.Controls.Add(this.txtName);
            this.gbMemberInfo.Controls.Add(this.lblPhoneNumber);
            this.gbMemberInfo.Controls.Add(this.txtPhoneNumber);
            this.gbMemberInfo.Controls.Add(this.lblRole);
            this.gbMemberInfo.Controls.Add(this.cmbRole);
            this.gbMemberInfo.Location = new System.Drawing.Point(15, 155); // Adjusted Y
            this.gbMemberInfo.Name = "gbMemberInfo";
            this.gbMemberInfo.Padding = new System.Windows.Forms.Padding(10);
            this.gbMemberInfo.Size = new System.Drawing.Size(420, 160); // Adjusted size
            this.gbMemberInfo.TabIndex = 1;
            this.gbMemberInfo.TabStop = false;
            this.gbMemberInfo.Text = "Member Information";
            //
            // lblName
            //
            this.lblName.Location = new System.Drawing.Point(15, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(125, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtName
            //
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtName.Location = new System.Drawing.Point(150, 30);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(250, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            //
            // lblPhoneNumber
            //
            this.lblPhoneNumber.Location = new System.Drawing.Point(15, 60);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(125, 23);
            this.lblPhoneNumber.TabIndex = 2;
            this.lblPhoneNumber.Text = "Phone Number:";
            this.lblPhoneNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtPhoneNumber
            //
            this.txtPhoneNumber.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtPhoneNumber.Location = new System.Drawing.Point(150, 60);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(250, 23);
            this.txtPhoneNumber.TabIndex = 3;
            this.txtPhoneNumber.Validating += new System.ComponentModel.CancelEventHandler(this.txtPhoneNumber_Validating);
            //
            // lblRole
            //
            this.lblRole.Location = new System.Drawing.Point(15, 90);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(125, 23);
            this.lblRole.TabIndex = 4;
            this.lblRole.Text = "Role:";
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRole.Visible = false;
            //
            // cmbRole
            //
            this.cmbRole.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Location = new System.Drawing.Point(150, 90);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(250, 23);
            this.cmbRole.TabIndex = 5;
            this.cmbRole.Visible = false;
            //
            // btnRegister
            //
            this.btnRegister.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRegister.Location = new System.Drawing.Point(175, 330); // Adjusted Y for new form height
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(100, 35);
            this.btnRegister.TabIndex = 2; // Form-level tab index
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            //
            // RegistrationForm
            //
            this.AcceptButton = this.btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(450, 380); // Adjusted ClientSize
            this.Controls.Add(this.gbAccountCredentials);
            this.Controls.Add(this.gbMemberInfo);
            this.Controls.Add(this.btnRegister);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Registration";
            this.Load += new System.EventHandler(this.RegistrationForm_Load);
            this.gbAccountCredentials.ResumeLayout(false);
            this.gbAccountCredentials.PerformLayout();
            this.gbMemberInfo.ResumeLayout(false);
            this.gbMemberInfo.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
