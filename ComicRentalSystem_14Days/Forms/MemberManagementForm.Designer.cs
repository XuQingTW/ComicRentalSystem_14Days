﻿namespace ComicRentalSystem_14Days.Forms
{
    partial class MemberManagementForm
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
            dgvMembers = new DataGridView();
            panel1 = new Panel();
            btnRefreshMembers = new Button();
            btnDeleteMember = new Button();
            btnEditMember = new Button();
            btnAddMember = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvMembers).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvMembers
            // 
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMembers.Dock = DockStyle.Fill;
            dgvMembers.Location = new Point(0, 0);
            dgvMembers.Name = "dgvMembers";
            dgvMembers.Size = new Size(800, 450);
            dgvMembers.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(btnRefreshMembers);
            panel1.Controls.Add(btnDeleteMember);
            panel1.Controls.Add(btnEditMember);
            panel1.Controls.Add(btnAddMember);
            panel1.Location = new Point(201, 376);
            panel1.Name = "panel1";
            panel1.Size = new Size(405, 62);
            panel1.TabIndex = 1;
            // 
            // btnRefreshMembers
            // 
            btnRefreshMembers.Location = new Point(291, 19);
            btnRefreshMembers.Name = "btnRefreshMembers";
            btnRefreshMembers.Size = new Size(75, 23);
            btnRefreshMembers.TabIndex = 3;
            btnRefreshMembers.Text = "重新整理";
            btnRefreshMembers.UseVisualStyleBackColor = true;
            // 
            // btnDeleteMember
            // 
            btnDeleteMember.Location = new Point(210, 19);
            btnDeleteMember.Name = "btnDeleteMember";
            btnDeleteMember.Size = new Size(75, 23);
            btnDeleteMember.TabIndex = 2;
            btnDeleteMember.Text = "刪除選定會員";
            btnDeleteMember.UseVisualStyleBackColor = true;
            this.btnDeleteMember.Click += new System.EventHandler(this.btnDeleteMember_Click);
            // 
            // btnEditMember
            // 
            btnEditMember.Location = new Point(129, 19);
            btnEditMember.Name = "btnEditMember";
            btnEditMember.Size = new Size(75, 23);
            btnEditMember.TabIndex = 1;
            btnEditMember.Text = "編輯選定會員";
            btnEditMember.UseVisualStyleBackColor = true;
            // 
            // btnAddMember
            // 
            btnAddMember.Location = new Point(48, 19);
            btnAddMember.Name = "btnAddMember";
            btnAddMember.Size = new Size(75, 23);
            btnAddMember.TabIndex = 0;
            btnAddMember.Text = "新增會員";
            btnAddMember.UseVisualStyleBackColor = true;
            btnAddMember.Click += btnAddMember_Click;
            // 
            // MemberManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(dgvMembers);
            Name = "MemberManagementForm";
            Text = "MemberManagementForm";
            ((System.ComponentModel.ISupportInitialize)dgvMembers).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvMembers;
        private Panel panel1;
        private Button btnRefreshMembers;
        private Button btnDeleteMember;
        private Button btnEditMember;
        private Button btnAddMember;
    }
}