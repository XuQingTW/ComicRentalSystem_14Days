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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.lblIsbn = new System.Windows.Forms.Label();
            this.txtIsbn = new System.Windows.Forms.TextBox();
            this.lblGenre = new System.Windows.Forms.Label();
            this.txtGenre = new System.Windows.Forms.TextBox();
            this.chkIsRented = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbComicDetails = new System.Windows.Forms.GroupBox();
            this.gbStatus = new System.Windows.Forms.GroupBox();

            this.gbComicDetails.SuspendLayout();
            this.gbStatus.SuspendLayout();
            this.SuspendLayout();
            //
            // gbComicDetails
            //
            this.gbComicDetails.Controls.Add(this.lblTitle);
            this.gbComicDetails.Controls.Add(this.txtTitle);
            this.gbComicDetails.Controls.Add(this.lblAuthor);
            this.gbComicDetails.Controls.Add(this.txtAuthor);
            this.gbComicDetails.Controls.Add(this.lblIsbn);
            this.gbComicDetails.Controls.Add(this.txtIsbn);
            this.gbComicDetails.Controls.Add(this.lblGenre);
            this.gbComicDetails.Controls.Add(this.txtGenre);
            this.gbComicDetails.Location = new System.Drawing.Point(15, 15);
            this.gbComicDetails.Name = "gbComicDetails";
            this.gbComicDetails.Padding = new System.Windows.Forms.Padding(10);
            this.gbComicDetails.Size = new System.Drawing.Size(450, 155); // Adjusted height
            this.gbComicDetails.TabIndex = 0;
            this.gbComicDetails.TabStop = false;
            this.gbComicDetails.Text = "Comic Information";
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(15, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title:";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtTitle.Location = new System.Drawing.Point(120, 30);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(315, 23);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Validating += new System.ComponentModel.CancelEventHandler(this.txtTitle_Validating);
            // 
            // lblAuthor
            // 
            this.lblAuthor.Location = new System.Drawing.Point(15, 60);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(95, 23);
            this.lblAuthor.TabIndex = 2;
            this.lblAuthor.Text = "Author:";
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAuthor
            // 
            this.txtAuthor.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtAuthor.Location = new System.Drawing.Point(120, 60);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(315, 23);
            this.txtAuthor.TabIndex = 3;
            this.txtAuthor.Validating += new System.ComponentModel.CancelEventHandler(this.txtAuthor_Validating);
            // 
            // lblIsbn
            // 
            this.lblIsbn.Location = new System.Drawing.Point(15, 90);
            this.lblIsbn.Name = "lblIsbn";
            this.lblIsbn.Size = new System.Drawing.Size(95, 23);
            this.lblIsbn.TabIndex = 4;
            this.lblIsbn.Text = "ISBN:";
            this.lblIsbn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtIsbn
            // 
            this.txtIsbn.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtIsbn.Location = new System.Drawing.Point(120, 90);
            this.txtIsbn.Name = "txtIsbn";
            this.txtIsbn.Size = new System.Drawing.Size(315, 23);
            this.txtIsbn.TabIndex = 5;
            this.txtIsbn.Validating += new System.ComponentModel.CancelEventHandler(this.txtIsbn_Validating);
            // 
            // lblGenre
            // 
            this.lblGenre.Location = new System.Drawing.Point(15, 120);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Size = new System.Drawing.Size(95, 23);
            this.lblGenre.TabIndex = 6;
            this.lblGenre.Text = "Genre:";
            this.lblGenre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGenre
            // 
            this.txtGenre.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtGenre.Location = new System.Drawing.Point(120, 120);
            this.txtGenre.Name = "txtGenre";
            this.txtGenre.Size = new System.Drawing.Size(315, 23);
            this.txtGenre.TabIndex = 7;
            this.txtGenre.Validating += new System.ComponentModel.CancelEventHandler(this.txtGenre_Validating);
            //
            // gbStatus
            //
            this.gbStatus.Controls.Add(this.chkIsRented);
            this.gbStatus.Location = new System.Drawing.Point(15, 180); // Positioned below gbComicDetails
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Padding = new System.Windows.Forms.Padding(10);
            this.gbStatus.Size = new System.Drawing.Size(450, 60);
            this.gbStatus.TabIndex = 1;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Rental Status";
            // 
            // chkIsRented
            // 
            this.chkIsRented.AutoSize = true;
            this.chkIsRented.Enabled = false;
            this.chkIsRented.Location = new System.Drawing.Point(15, 25);
            this.chkIsRented.Name = "chkIsRented";
            this.chkIsRented.Size = new System.Drawing.Size(100, 19); // Adjusted size
            this.chkIsRented.TabIndex = 0; // TabIndex within this groupbox
            this.chkIsRented.Text = "Is Rented";
            this.chkIsRented.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnSave.Location = new System.Drawing.Point(279, 262); // Positioned at bottom right
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnCancel.Location = new System.Drawing.Point(375, 262); // Positioned at bottom right
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ComicEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(480, 307); // Adjusted client size
            this.Controls.Add(this.gbComicDetails);
            this.Controls.Add(this.gbStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "ComicEditForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "Edit Comic"; // Default title
            this.gbComicDetails.ResumeLayout(false);
            this.gbComicDetails.PerformLayout();
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label lblIsbn;
        private System.Windows.Forms.TextBox txtIsbn;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.TextBox txtGenre;
        private System.Windows.Forms.CheckBox chkIsRented;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbComicDetails;
        private System.Windows.Forms.GroupBox gbStatus;
    });
}