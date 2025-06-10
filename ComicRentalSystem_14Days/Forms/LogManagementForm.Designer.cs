namespace ComicRentalSystem_14Days.Forms
{
    partial class LogManagementForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listViewLogs;
        private System.Windows.Forms.NumericUpDown numericRetentionDays;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDeleteSelected;

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
            this.listViewLogs = new System.Windows.Forms.ListView();
            this.numericRetentionDays = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericRetentionDays)).BeginInit();
            this.SuspendLayout();
            this.listViewLogs.FullRowSelect = true;
            this.listViewLogs.HideSelection = false;
            this.listViewLogs.Location = new System.Drawing.Point(12, 12);
            this.listViewLogs.MultiSelect = true;
            this.listViewLogs.Name = "listViewLogs";
            this.listViewLogs.Size = new System.Drawing.Size(360, 200);
            this.listViewLogs.View = System.Windows.Forms.View.List;
            this.numericRetentionDays.Location = new System.Drawing.Point(12, 222);
            this.numericRetentionDays.Maximum = new decimal(new int[] {365,0,0,0});
            this.numericRetentionDays.Minimum = new decimal(new int[] {1,0,0,0});
            this.numericRetentionDays.Name = "numericRetentionDays";
            this.numericRetentionDays.Size = new System.Drawing.Size(120, 23);
            this.numericRetentionDays.Value = new decimal(new int[] {90,0,0,0});
            this.btnSave.Location = new System.Drawing.Point(150, 222);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.Text = "保存設定";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnDeleteSelected.Location = new System.Drawing.Point(272, 222);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(100, 23);
            this.btnDeleteSelected.Text = "刪除選取";
            this.btnDeleteSelected.UseVisualStyleBackColor = true;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.btnDeleteSelected);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numericRetentionDays);
            this.Controls.Add(this.listViewLogs);
            this.Name = "LogManagementForm";
            this.Text = "日誌管理";
            ((System.ComponentModel.ISupportInitialize)(this.numericRetentionDays)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
