namespace ComicRentalSystem_14Days.Forms
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
            btnChangeUserRole = new Button(); // Added declaration for the new button
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
            dgvMembers.Size = new Size(700, 397);
            dgvMembers.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(btnChangeUserRole); // Added the new button to the panel
            panel1.Controls.Add(btnRefreshMembers);
            panel1.Controls.Add(btnDeleteMember);
            panel1.Controls.Add(btnEditMember);
            panel1.Controls.Add(btnAddMember);
            panel1.Location = new Point(12, 332); // Adjusted panel location for better layout
            panel1.Name = "panel1";
            panel1.Size = new Size(676, 55); // Adjusted panel size for better layout
            panel1.TabIndex = 1;
            // 
            // btnRefreshMembers
            // 
            btnRefreshMembers.Location = new Point(360, 17); // Adjusted location
            btnRefreshMembers.Name = "btnRefreshMembers";
            btnRefreshMembers.Size = new System.Drawing.Size(75, 23); // Adjusted size
            btnRefreshMembers.TabIndex = 3;
            btnRefreshMembers.Text = "重新整理";
            btnRefreshMembers.UseVisualStyleBackColor = true;
            // btnRefreshMembers.Click += btnRefreshMembers_Click; // Event handler already wired up in .cs
            // 
            // btnDeleteMember
            // 
            btnDeleteMember.Location = new Point(279, 17); // Adjusted location
            btnDeleteMember.Name = "btnDeleteMember";
            btnDeleteMember.Size = new System.Drawing.Size(75, 23); // Adjusted size
            btnDeleteMember.TabIndex = 2;
            btnDeleteMember.Text = "刪除選定會員";
            btnDeleteMember.UseVisualStyleBackColor = true;
            btnDeleteMember.Click += btnDeleteMember_Click;
            // 
            // btnEditMember
            // 
            btnEditMember.Location = new Point(198, 17); // Adjusted location
            btnEditMember.Name = "btnEditMember";
            btnEditMember.Size = new System.Drawing.Size(75, 23); // Adjusted size
            btnEditMember.TabIndex = 1;
            btnEditMember.Text = "編輯選定會員";
            btnEditMember.UseVisualStyleBackColor = true;
            btnEditMember.Click += btnEditMember_Click;
            // 
            // btnAddMember
            // 
            btnAddMember.Location = new Point(117, 17); // Adjusted location
            btnAddMember.Name = "btnAddMember";
            btnAddMember.Size = new System.Drawing.Size(75, 23); // Adjusted size
            btnAddMember.TabIndex = 0;
            btnAddMember.Text = "新增會員";
            btnAddMember.UseVisualStyleBackColor = true;
            btnAddMember.Click += btnAddMember_Click;
            //
            // btnChangeUserRole
            //
            btnChangeUserRole.Location = new Point(36, 17); // Positioned as the first button
            btnChangeUserRole.Name = "btnChangeUserRole";
            btnChangeUserRole.Size = new System.Drawing.Size(75, 23); // Standardized size
            btnChangeUserRole.TabIndex = 4; // New tab index
            btnChangeUserRole.Text = "更改使用者角色";
            btnChangeUserRole.UseVisualStyleBackColor = true;
            // btnChangeUserRole.Click += btnChangeUserRole_Click; // This will be wired up by MemberManagementForm_Load
            // 
            // MemberManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 397);
            Controls.Add(panel1);
            Controls.Add(dgvMembers);
            Name = "MemberManagementForm";
            Text = "MemberManagementForm";
            Load += MemberManagementForm_Load;
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
        private Button btnChangeUserRole; // Added field for the new button
    }
}