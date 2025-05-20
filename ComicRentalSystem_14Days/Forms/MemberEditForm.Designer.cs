namespace ComicRentalSystem_14Days.Forms
{
    partial class MemberEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblName = new Label();
            lblPhoneNumber = new Label();
            txtName = new TextBox();
            txtPhoneNumber = new TextBox();
            btnSaveMember = new Button();
            btnCancelMember = new Button();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(242, 41);
            lblName.Name = "lblName";
            lblName.Size = new Size(36, 18);
            lblName.TabIndex = 0;
            lblName.Text = "姓名";
            // 
            // lblPhoneNumber
            // 
            lblPhoneNumber.AutoSize = true;
            lblPhoneNumber.Location = new Point(214, 68);
            lblPhoneNumber.Name = "lblPhoneNumber";
            lblPhoneNumber.Size = new Size(64, 18);
            lblPhoneNumber.TabIndex = 1;
            lblPhoneNumber.Text = "電話號碼";
            // 
            // txtName
            // 
            txtName.Location = new Point(284, 35);
            txtName.Name = "txtName";
            txtName.Size = new Size(100, 24);
            txtName.TabIndex = 2;
            // 
            // txtPhoneNumber
            // 
            txtPhoneNumber.Location = new Point(284, 65);
            txtPhoneNumber.Name = "txtPhoneNumber";
            txtPhoneNumber.Size = new Size(100, 24);
            txtPhoneNumber.TabIndex = 3;
            // 
            // btnSaveMember
            // 
            btnSaveMember.Location = new Point(295, 95);
            btnSaveMember.Name = "btnSaveMember";
            btnSaveMember.Size = new Size(75, 23);
            btnSaveMember.TabIndex = 4;
            btnSaveMember.Text = "儲存";
            btnSaveMember.UseVisualStyleBackColor = true;
            btnSaveMember.Click += new System.EventHandler(this.btnSaveMember_Click);
            // 
            // btnCancelMember
            // 
            btnCancelMember.Location = new Point(214, 98);
            btnCancelMember.Name = "btnCancelMember";
            btnCancelMember.Size = new Size(75, 23);
            btnCancelMember.TabIndex = 5;
            btnCancelMember.Text = "取消";
            btnCancelMember.UseVisualStyleBackColor = true;
            btnCancelMember.Click += new System.EventHandler(this.btnCancelMember_Click);
            // 
            // MemberEditForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCancelMember);
            Controls.Add(btnSaveMember);
            Controls.Add(txtPhoneNumber);
            Controls.Add(txtName);
            Controls.Add(lblPhoneNumber);
            Controls.Add(lblName);
            Name = "MemberEditForm";
            Text = "MemberEditForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblName;
        private Label lblPhoneNumber;
        private TextBox txtName;
        private TextBox txtPhoneNumber;
        private Button btnSaveMember;
        private Button btnCancelMember;
    }
}