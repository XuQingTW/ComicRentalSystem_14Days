namespace ComicRentalSystem_14Days.Forms
{
    partial class RentalPeriodForm
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
            this.monthCalendarRental = new System.Windows.Forms.MonthCalendar();
            this.btnConfirmRental = new System.Windows.Forms.Button();
            this.btnCancelRental = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // monthCalendarRental
            //
            this.monthCalendarRental.Location = new System.Drawing.Point(18, 50);
            this.monthCalendarRental.MaxSelectionCount = 1; // Important for selecting a single date
            this.monthCalendarRental.Name = "monthCalendarRental";
            this.monthCalendarRental.TabIndex = 0;
            //
            // btnConfirmRental
            //
            this.btnConfirmRental.Location = new System.Drawing.Point(18, 225);
            this.btnConfirmRental.Name = "btnConfirmRental";
            this.btnConfirmRental.Size = new System.Drawing.Size(100, 30);
            this.btnConfirmRental.TabIndex = 1;
            this.btnConfirmRental.Text = "Confirm";
            this.btnConfirmRental.UseVisualStyleBackColor = true;
            this.btnConfirmRental.Click += new System.EventHandler(this.btnConfirmRental_Click);
            //
            // btnCancelRental
            //
            this.btnCancelRental.Location = new System.Drawing.Point(145, 225);
            this.btnCancelRental.Name = "btnCancelRental";
            this.btnCancelRental.Size = new System.Drawing.Size(100, 30);
            this.btnCancelRental.TabIndex = 2;
            this.btnCancelRental.Text = "Cancel";
            this.btnCancelRental.UseVisualStyleBackColor = true;
            this.btnCancelRental.Click += new System.EventHandler(this.btnCancelRental_Click);
            //
            // lblInfo
            //
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(18, 15);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(220, 15);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "Select a return date (Min 3 days, Max 1 month).";
            //
            // RentalPeriodForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 271);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnCancelRental);
            this.Controls.Add(this.btnConfirmRental);
            this.Controls.Add(this.monthCalendarRental);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RentalPeriodForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Rental Period";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendarRental;
        private System.Windows.Forms.Button btnConfirmRental;
        private System.Windows.Forms.Button btnCancelRental;
        private System.Windows.Forms.Label lblInfo;
    }
}
