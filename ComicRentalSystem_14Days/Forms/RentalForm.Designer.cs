namespace ComicRentalSystem_14Days.Forms
{
    partial class RentalForm
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
            cmbMembers = new ComboBox();
            cmbComics = new ComboBox();
            btnReturn = new Button();
            btnRent = new Button();
            dgvRentedComics = new DataGridView();
            dtpActualReturnTime = new DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).BeginInit();
            SuspendLayout();
            // 
            // cmbMembers
            // 
            cmbMembers.FormattingEnabled = true;
            cmbMembers.Location = new Point(11, 12);
            cmbMembers.Margin = new Padding(2, 3, 2, 3);
            cmbMembers.Name = "cmbMembers";
            cmbMembers.Size = new Size(86, 23);
            cmbMembers.TabIndex = 0;
            cmbMembers.Text = "選擇會員";
            cmbMembers.SelectedIndexChanged += cmbMembers_SelectedIndexChanged;
            // 
            // cmbComics
            // 
            cmbComics.FormattingEnabled = true;
            cmbComics.Location = new Point(115, 12);
            cmbComics.Margin = new Padding(2, 3, 2, 3);
            cmbComics.Name = "cmbComics";
            cmbComics.Size = new Size(86, 23);
            cmbComics.TabIndex = 1;
            cmbComics.Text = "選擇漫畫";
            // 
            // btnReturn
            // 
            btnReturn.Location = new Point(261, 12);
            btnReturn.Margin = new Padding(2, 3, 2, 3);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(52, 23);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "歸還";
            btnReturn.UseVisualStyleBackColor = true;
            btnReturn.Click += btnReturn_Click;
            // 
            // btnRent
            // 
            btnRent.Location = new Point(205, 12);
            btnRent.Margin = new Padding(2, 3, 2, 3);
            btnRent.Name = "btnRent";
            btnRent.Size = new Size(52, 24);
            btnRent.TabIndex = 4;
            btnRent.Text = "租借";
            btnRent.UseVisualStyleBackColor = true;
            btnRent.Click += btnRent_Click;
            // 
            // dgvRentedComics
            // 
            dgvRentedComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentedComics.Location = new Point(10, 55);
            dgvRentedComics.Margin = new Padding(2, 3, 2, 3);
            dgvRentedComics.Name = "dgvRentedComics";
            dgvRentedComics.RowHeadersWidth = 51;
            dgvRentedComics.Size = new Size(704, 415);
            dgvRentedComics.TabIndex = 5;
            // 
            // dtpActualReturnTime
            // 
            dtpActualReturnTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dtpActualReturnTime.Format = DateTimePickerFormat.Custom;
            dtpActualReturnTime.Location = new Point(317, 13);
            dtpActualReturnTime.Margin = new Padding(2, 3, 2, 3);
            dtpActualReturnTime.Name = "dtpActualReturnTime";
            dtpActualReturnTime.Size = new Size(132, 23);
            dtpActualReturnTime.TabIndex = 3;
            // 
            // RentalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(725, 482);
            Controls.Add(dtpActualReturnTime);
            Controls.Add(dgvRentedComics);
            Controls.Add(btnRent);
            Controls.Add(btnReturn);
            Controls.Add(cmbComics);
            Controls.Add(cmbMembers);
            Margin = new Padding(2, 3, 2, 3);
            Name = "RentalForm";
            Text = "RentalForm";
            Load += RentalForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbMembers;
        private System.Windows.Forms.ComboBox cmbComics;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Button btnRent;
        private System.Windows.Forms.DataGridView dgvRentedComics; // CORRECTED
        private System.Windows.Forms.DateTimePicker dtpActualReturnTime; // Added
    }


}