namespace ComicRentalSystem_14Days.Forms
{
    partial class ComicEditForm
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
            lblTitle = new Label();
            lblAuthor = new Label();
            lblIsbn = new Label();
            lblGenre = new Label();
            txtAuthor = new TextBox();
            txtIsbn = new TextBox();
            txtGenre = new TextBox();
            chkIsRented = new CheckBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            txtTitle = new TextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnSave = new Button();
            btnCancel = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(3, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(36, 18);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "書名";
            // 
            // lblAuthor
            // 
            lblAuthor.AutoSize = true;
            lblAuthor.Location = new Point(3, 29);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(36, 18);
            lblAuthor.TabIndex = 1;
            lblAuthor.Text = "作者";
            // 
            // lblIsbn
            // 
            lblIsbn.AutoSize = true;
            lblIsbn.Location = new Point(3, 0);
            lblIsbn.Name = "lblIsbn";
            lblIsbn.Size = new Size(40, 18);
            lblIsbn.TabIndex = 2;
            lblIsbn.Text = "ISBN";
            // 
            // lblGenre
            // 
            lblGenre.AutoSize = true;
            lblGenre.Location = new Point(3, 28);
            lblGenre.Name = "lblGenre";
            lblGenre.Size = new Size(36, 18);
            lblGenre.TabIndex = 3;
            lblGenre.Text = "類型";
            // 
            // txtAuthor
            // 
            txtAuthor.Location = new Point(50, 32);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Size = new Size(100, 24);
            txtAuthor.TabIndex = 5;
            // 
            // txtIsbn
            // 
            txtIsbn.Location = new Point(52, 3);
            txtIsbn.Name = "txtIsbn";
            txtIsbn.Size = new Size(100, 24);
            txtIsbn.TabIndex = 6;
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(52, 31);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(100, 24);
            txtGenre.TabIndex = 7;
            // 
            // chkIsRented
            // 
            chkIsRented.AutoSize = true;
            chkIsRented.Enabled = false;
            chkIsRented.Location = new Point(280, 194);
            chkIsRented.Name = "chkIsRented";
            chkIsRented.Size = new Size(97, 22);
            chkIsRented.TabIndex = 8;
            chkIsRented.Text = "是否已租借";
            chkIsRented.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.1740417F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80.82596F));
            tableLayoutPanel1.Controls.Add(lblAuthor, 0, 1);
            tableLayoutPanel1.Controls.Add(lblTitle, 0, 0);
            tableLayoutPanel1.Controls.Add(txtTitle, 1, 0);
            tableLayoutPanel1.Controls.Add(txtAuthor, 1, 1);
            tableLayoutPanel1.Location = new Point(277, 67);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(250, 58);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(50, 3);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(100, 24);
            txtTitle.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.6428566F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80.35714F));
            tableLayoutPanel2.Controls.Add(txtIsbn, 1, 0);
            tableLayoutPanel2.Controls.Add(lblIsbn, 0, 0);
            tableLayoutPanel2.Controls.Add(lblGenre, 0, 1);
            tableLayoutPanel2.Controls.Add(txtGenre, 1, 1);
            tableLayoutPanel2.Location = new Point(277, 131);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(250, 57);
            tableLayoutPanel2.TabIndex = 10;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(373, 192);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 11;
            btnSave.Text = "儲存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(452, 192);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // ComicEditForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(chkIsRented);
            Name = "ComicEditForm";
            Text = "ComicEditForm";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblAuthor;
        private Label lblIsbn;
        private Label lblGenre;
        private TextBox txtAuthor;
        private TextBox txtIsbn;
        private TextBox txtGenre;
        private CheckBox chkIsRented;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtTitle;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnSave;
        private Button btnCancel;
    }
}