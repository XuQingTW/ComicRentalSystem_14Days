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
            lblMember = new Label();
            lblComic = new Label();
            lblActualReturnTime = new Label();
            lblRentedComics = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).BeginInit();
            SuspendLayout();
            // 
            // lblMember
            //
            lblMember.AutoSize = true;
            lblMember.Location = new Point(11, 16);
            lblMember.Name = "lblMember";
            lblMember.Size = new Size(44, 15);
            lblMember.TabIndex = 6;
            lblMember.Text = "會員:";
            //
            // cmbMembers
            // 
            cmbMembers.FormattingEnabled = true;
            cmbMembers.Location = new Point(60, 12); // Adjusted X
            cmbMembers.Margin = new Padding(2, 3, 2, 3);
            cmbMembers.Name = "cmbMembers";
            cmbMembers.Size = new Size(120, 23); // Adjusted Size
            cmbMembers.TabIndex = 0;
            // cmbMembers.Text = "選擇會員"; // Initial text can be removed if populated on load
            cmbMembers.SelectedIndexChanged += cmbMembers_SelectedIndexChanged;
            // 
            // lblComic
            //
            lblComic.AutoSize = true;
            lblComic.Location = new Point(190, 16); // Adjusted X
            lblComic.Name = "lblComic";
            lblComic.Size = new Size(44, 15);
            lblComic.TabIndex = 7;
            lblComic.Text = "漫畫:";
            //
            // cmbComics
            // 
            cmbComics.FormattingEnabled = true;
            cmbComics.Location = new Point(239, 12); // Adjusted X
            cmbComics.Margin = new Padding(2, 3, 2, 3);
            cmbComics.Name = "cmbComics";
            cmbComics.Size = new Size(120, 23); // Adjusted Size
            cmbComics.TabIndex = 1;
            // cmbComics.Text = "選擇漫畫"; // Initial text can be removed
            // 
            // btnRent
            // 
            btnRent.Location = new Point(370, 11); // Adjusted X
            btnRent.Margin = new Padding(2, 3, 2, 3);
            btnRent.Name = "btnRent";
            btnRent.Size = new Size(52, 24);
            btnRent.TabIndex = 4;
            btnRent.Text = "租借";
            btnRent.UseVisualStyleBackColor = true;
            btnRent.Click += btnRent_Click;
            // 
            // lblActualReturnTime
            // 
            lblActualReturnTime.AutoSize = true;
            lblActualReturnTime.Location = new Point(432, 16); // Adjusted X
            lblActualReturnTime.Name = "lblActualReturnTime";
            lblActualReturnTime.Size = new Size(92, 15);
            lblActualReturnTime.TabIndex = 8;
            lblActualReturnTime.Text = "實際歸還時間:";
            // 
            // dtpActualReturnTime
            // 
            dtpActualReturnTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dtpActualReturnTime.Format = DateTimePickerFormat.Custom;
            dtpActualReturnTime.Location = new Point(528, 12); // Adjusted X
            dtpActualReturnTime.Margin = new Padding(2, 3, 2, 3);
            dtpActualReturnTime.Name = "dtpActualReturnTime";
            dtpActualReturnTime.Size = new Size(150, 23); // Adjusted Size
            dtpActualReturnTime.TabIndex = 3;
            // 
            // btnReturn
            //
            btnReturn.Location = new Point(684, 11); // Adjusted X
            btnReturn.Margin = new Padding(2, 3, 2, 3);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(52, 23);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "歸還";
            btnReturn.UseVisualStyleBackColor = true;
            btnReturn.Click += btnReturn_Click;
            //
            // lblRentedComics
            //
            lblRentedComics.AutoSize = true;
            lblRentedComics.Location = new Point(10, 45); // Positioned above dgvRentedComics
            lblRentedComics.Name = "lblRentedComics";
            lblRentedComics.Size = new Size(92, 15);
            lblRentedComics.TabIndex = 9;
            lblRentedComics.Text = "目前租借記錄:";
            //
            // dgvRentedComics
            //
            dgvRentedComics.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right; // Added Anchor for resizing
            dgvRentedComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentedComics.Location = new Point(10, 65); // Adjusted Y
            dgvRentedComics.Margin = new Padding(2, 3, 2, 3);
            dgvRentedComics.Name = "dgvRentedComics";
            dgvRentedComics.RowHeadersWidth = 51;
            dgvRentedComics.Size = new Size(704, 405); // Adjusted Height
            dgvRentedComics.TabIndex = 5;
            //
            // RentalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(745, 482); // Adjusted ClientSize
            Controls.Add(lblRentedComics);
            Controls.Add(lblActualReturnTime);
            Controls.Add(lblComic);
            Controls.Add(lblMember);
            Controls.Add(dtpActualReturnTime);
            Controls.Add(dgvRentedComics);
            Controls.Add(btnRent);
            Controls.Add(btnReturn);
            Controls.Add(cmbComics);
            Controls.Add(cmbMembers);
            Margin = new Padding(2, 3, 2, 3);
            Name = "RentalForm";
            Text = "租借管理"; // Changed Form Title
            Load += RentalForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).EndInit();
            ResumeLayout(false);
            PerformLayout(); // Added to apply AutoSize of labels

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