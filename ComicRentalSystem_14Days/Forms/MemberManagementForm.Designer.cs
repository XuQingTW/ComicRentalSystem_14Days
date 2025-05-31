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
            btnChangeUserRole = new Button();
            txtSearchMembers = new TextBox();
            btnSearchMembers = new Button();
            btnClearSearchMembers = new Button();
            panelSearch = new Panel(); // Panel for search controls
            ((System.ComponentModel.ISupportInitialize)dgvMembers).BeginInit();
            panel1.SuspendLayout();
            panelSearch.SuspendLayout();
            SuspendLayout();
            // 
            // panelSearch
            //
            panelSearch.Controls.Add(txtSearchMembers);
            panelSearch.Controls.Add(btnSearchMembers);
            panelSearch.Controls.Add(btnClearSearchMembers);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(0, 0);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(700, 30);
            panelSearch.TabIndex = 2;
            //
            // txtSearchMembers
            //
            txtSearchMembers.Location = new Point(12, 3);
            txtSearchMembers.Name = "txtSearchMembers";
            txtSearchMembers.Size = new Size(200, 23);
            txtSearchMembers.TabIndex = 0;
            //
            // btnSearchMembers
            //
            btnSearchMembers.Location = new Point(218, 2);
            btnSearchMembers.Name = "btnSearchMembers";
            btnSearchMembers.Size = new Size(75, 25);
            btnSearchMembers.TabIndex = 1;
            btnSearchMembers.Text = "搜尋"; // Search
            btnSearchMembers.UseVisualStyleBackColor = true;
            // Event handler btnSearchMembers_Click to be connected in MemberManagementForm.cs
            //
            // btnClearSearchMembers
            //
            btnClearSearchMembers.Location = new Point(299, 2);
            btnClearSearchMembers.Name = "btnClearSearchMembers";
            btnClearSearchMembers.Size = new Size(75, 25);
            btnClearSearchMembers.TabIndex = 2;
            btnClearSearchMembers.Text = "清除"; // Clear
            btnClearSearchMembers.UseVisualStyleBackColor = true;
            // Event handler btnClearSearchMembers_Click to be connected in MemberManagementForm.cs
            //
            // dgvMembers
            // 
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMembers.Dock = DockStyle.Fill;
            dgvMembers.Location = new Point(0, 30);
            dgvMembers.Name = "dgvMembers";
            dgvMembers.Size = new Size(700, 312);
            dgvMembers.TabIndex = 0;
            // dgvMembers.CellDoubleClick += dgvMembers_CellDoubleClick; // This is already present and correct
            // 
            // panel1
            // 
            panel1.Controls.Add(btnChangeUserRole);
            panel1.Controls.Add(btnRefreshMembers);
            panel1.Controls.Add(btnDeleteMember);
            panel1.Controls.Add(btnEditMember);
            panel1.Controls.Add(btnAddMember);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 342);
            panel1.Name = "panel1";
            panel1.Size = new Size(700, 55);
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
            Controls.Add(dgvMembers);
            Controls.Add(panel1);
            Controls.Add(panelSearch);
            Name = "MemberManagementForm";
            Text = "會員管理"; // Changed form title to "會員管理" (Member Management)
            Load += MemberManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMembers).EndInit();
            panel1.ResumeLayout(false);
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvMembers;
        private Panel panel1;
        private Button btnRefreshMembers;
        private Button btnDeleteMember;
        private Button btnEditMember;
        private Button btnAddMember;
        private Button btnChangeUserRole;
        private Panel panelSearch;
        private TextBox txtSearchMembers;
        private Button btnSearchMembers;
        private Button btnClearSearchMembers;
    }
}