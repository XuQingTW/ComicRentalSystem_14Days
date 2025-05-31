namespace ComicRentalSystem_14Days.Controls
{
    partial class AdminDashboardUserControl
    {
        private System.ComponentModel.IContainer components = null;

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
            var gbTotalComics = new System.Windows.Forms.GroupBox();
            // this.lblTotalComicsValue is declared in AdminDashboardUserControl.cs
            var gbRentedComics = new System.Windows.Forms.GroupBox();
            // this.lblRentedComicsValue is declared in AdminDashboardUserControl.cs
            var gbAvailableComics = new System.Windows.Forms.GroupBox();
            // this.lblAvailableComicsValue is declared in AdminDashboardUserControl.cs
            var gbActiveMembers = new System.Windows.Forms.GroupBox();
            // this.lblActiveMembersValue is declared in AdminDashboardUserControl.cs
            var lblDashboardTitle = new System.Windows.Forms.Label();

            // Ensure labels are instantiated if not already by base class or if they are not fields.
            // Based on AdminDashboardUserControl.cs, these are fields, so they should be available.
            // However, designer typically initializes them here if they are part of this UserControl's design surface.
            // The provided C# makes them internal fields, initialized by the UserControl's logic,
            // but the designer needs to create them to add them to GroupBoxes.
            // Let's assume they are instantiated in AdminDashboardUserControl.cs and then added to controls here.
            // For the designer to correctly wire them up if they were dropped on a form, they'd be declared here.
            // Given the structure, we will instantiate them here for the designer's context.
            this.lblTotalComicsValue = new System.Windows.Forms.Label();
            this.lblRentedComicsValue = new System.Windows.Forms.Label();
            this.lblAvailableComicsValue = new System.Windows.Forms.Label();
            this.lblActiveMembersValue = new System.Windows.Forms.Label();

            this.SuspendLayout();

            lblDashboardTitle.AutoSize = true;
            lblDashboardTitle.Font = ModernBaseForm.HeadingFont ?? new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            lblDashboardTitle.ForeColor = ModernBaseForm.PrimaryColor;
            lblDashboardTitle.Location = new System.Drawing.Point(15, 10);
            lblDashboardTitle.Name = "lblDashboardTitle";
            lblDashboardTitle.Size = new System.Drawing.Size(250, 30); // Approximate size
            lblDashboardTitle.Text = "Dashboard Overview";

            System.Drawing.Font metricValueFont = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            System.Drawing.Color metricValueColor = ModernBaseForm.TextColor;
            System.Windows.Forms.DockStyle metricLabelDockStyle = System.Windows.Forms.DockStyle.Fill;
            System.Drawing.ContentAlignment metricLabelContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            System.Drawing.Font groupBoxFont = ModernBaseForm.PrimaryFontBold ?? new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            System.Drawing.Color groupBoxForeColor = ModernBaseForm.TextColor;
            System.Windows.Forms.Padding groupBoxPadding = new System.Windows.Forms.Padding(10, 8, 10, 10);

            gbTotalComics.SuspendLayout();
            gbTotalComics.Controls.Add(this.lblTotalComicsValue);
            gbTotalComics.Font = groupBoxFont;
            gbTotalComics.ForeColor = groupBoxForeColor;
            gbTotalComics.Location = new System.Drawing.Point(20, 50);
            gbTotalComics.Name = "gbTotalComics";
            gbTotalComics.Size = new System.Drawing.Size(200, 120);
            gbTotalComics.Text = "Total Comics";
            gbTotalComics.Padding = groupBoxPadding;
            this.lblTotalComicsValue.Dock = metricLabelDockStyle;
            this.lblTotalComicsValue.Font = metricValueFont;
            this.lblTotalComicsValue.ForeColor = metricValueColor;
            this.lblTotalComicsValue.Name = "lblTotalComicsValue";
            this.lblTotalComicsValue.Text = "0";
            this.lblTotalComicsValue.TextAlign = metricLabelContentAlignment;
            gbTotalComics.ResumeLayout(false);

            gbRentedComics.SuspendLayout();
            gbRentedComics.Controls.Add(this.lblRentedComicsValue);
            gbRentedComics.Font = groupBoxFont;
            gbRentedComics.ForeColor = groupBoxForeColor;
            gbRentedComics.Location = new System.Drawing.Point(230, 50);
            gbRentedComics.Name = "gbRentedComics";
            gbRentedComics.Size = new System.Drawing.Size(200, 120);
            gbRentedComics.Text = "Rented Comics";
            gbRentedComics.Padding = groupBoxPadding;
            this.lblRentedComicsValue.Dock = metricLabelDockStyle;
            this.lblRentedComicsValue.Font = metricValueFont;
            this.lblRentedComicsValue.ForeColor = metricValueColor;
            this.lblRentedComicsValue.Name = "lblRentedComicsValue";
            this.lblRentedComicsValue.Text = "0";
            this.lblRentedComicsValue.TextAlign = metricLabelContentAlignment;
            gbRentedComics.ResumeLayout(false);

            gbAvailableComics.SuspendLayout();
            gbAvailableComics.Controls.Add(this.lblAvailableComicsValue);
            gbAvailableComics.Font = groupBoxFont;
            gbAvailableComics.ForeColor = groupBoxForeColor;
            gbAvailableComics.Location = new System.Drawing.Point(20, 180);
            gbAvailableComics.Name = "gbAvailableComics";
            gbAvailableComics.Size = new System.Drawing.Size(200, 120);
            gbAvailableComics.Text = "Available Comics";
            gbAvailableComics.Padding = groupBoxPadding;
            this.lblAvailableComicsValue.Dock = metricLabelDockStyle;
            this.lblAvailableComicsValue.Font = metricValueFont;
            this.lblAvailableComicsValue.ForeColor = metricValueColor;
            this.lblAvailableComicsValue.Name = "lblAvailableComicsValue";
            this.lblAvailableComicsValue.Text = "0";
            this.lblAvailableComicsValue.TextAlign = metricLabelContentAlignment;
            gbAvailableComics.ResumeLayout(false);

            gbActiveMembers.SuspendLayout();
            gbActiveMembers.Controls.Add(this.lblActiveMembersValue);
            gbActiveMembers.Font = groupBoxFont;
            gbActiveMembers.ForeColor = groupBoxForeColor;
            gbActiveMembers.Location = new System.Drawing.Point(230, 180);
            gbActiveMembers.Name = "gbActiveMembers";
            gbActiveMembers.Size = new System.Drawing.Size(200, 120);
            gbActiveMembers.Text = "Active Members";
            gbActiveMembers.Padding = groupBoxPadding;
            this.lblActiveMembersValue.Dock = metricLabelDockStyle;
            this.lblActiveMembersValue.Font = metricValueFont;
            this.lblActiveMembersValue.ForeColor = metricValueColor;
            this.lblActiveMembersValue.Name = "lblActiveMembersValue";
            this.lblActiveMembersValue.Text = "0";
            this.lblActiveMembersValue.TextAlign = metricLabelContentAlignment;
            gbActiveMembers.ResumeLayout(false);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = ModernBaseForm.SecondaryColor;
            this.Controls.Add(lblDashboardTitle);
            this.Controls.Add(gbTotalComics);
            this.Controls.Add(gbRentedComics);
            this.Controls.Add(gbAvailableComics);
            this.Controls.Add(gbActiveMembers);
            this.Name = "AdminDashboardUserControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(450, 320);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
