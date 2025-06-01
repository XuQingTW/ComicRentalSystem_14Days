// ModernBaseForm.cs

using System;
using System.Drawing;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Interfaces;  // ∑sºW°G§ﬁ§J ILogger ©w∏q©“¶b™∫©R¶W™≈∂°

namespace ComicRentalSystem_14Days
{
    public class ModernBaseForm : Form
    {
        // ÂÆöÁæ©Áèæ‰ª£Ëâ≤ÂΩ©Ë™øËâ≤Áõ§
        public static Color PrimaryColor { get; } = Color.FromArgb(0, 123, 255);
        public static Color SecondaryColor { get; } = Color.FromArgb(248, 249, 250);
        public static Color TextColor { get; } = Color.FromArgb(33, 37, 41);
        public static Color AccentColor { get; } = Color.FromArgb(255, 193, 7);
        public static Color SuccessColor { get; } = Color.FromArgb(40, 167, 69);
        public static Color DangerColor { get; } = Color.FromArgb(220, 53, 69);
        public static Color LightBorderColor { get; } = Color.FromArgb(222, 226, 230);

        // ÂÆöÁæ©Áèæ‰ª£Â≠óÂûã
        public static Font PrimaryFont { get; } = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        public static Font PrimaryFontBold { get; } = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
        public static Font ButtonFont { get; } = new Font("Segoe UI Semibold", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        public static Font HeadingFont { get; } = new Font("Segoe UI Semibold", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        public static Font DataGridViewHeaderFont { get; } = new Font("Segoe UI Semibold", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        public ModernBaseForm()
        {
            InitializeModernStyles();
        }

        private void InitializeModernStyles()
        {
            this.BackColor = SecondaryColor;
            this.ForeColor = TextColor;
            this.Font = PrimaryFont;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Padding = new Padding(10);
        }

        protected void StyleModernButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = PrimaryColor;
            btn.BackColor = PrimaryColor;
            btn.ForeColor = Color.White;
            btn.Font = ButtonFont;
            btn.Padding = new Padding(10, 0, 10, 0);
            btn.MinimumSize = new Size(120, 40);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
        }

        protected void StyleSecondaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = LightBorderColor;
            btn.BackColor = Color.White;
            btn.ForeColor = TextColor;
            btn.Font = ButtonFont;
            btn.Padding = new Padding(10, 0, 10, 0);
            btn.MinimumSize = new Size(120, 40);
            btn.TextAlign = ContentAlignment.MiddleCenter;
        }

        protected void StyleModernGroupBox(GroupBox gb)
        {
            gb.Font = HeadingFont;
            gb.ForeColor = TextColor;
            gb.Padding = new Padding(10, 5, 10, 10);
        }

        protected void StyleModernDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = SecondaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = DataGridViewHeaderFont;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.EnableHeadersVisualStyles = false;

            dgv.DefaultCellStyle.Font = PrimaryFont;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = AccentColor;
            dgv.DefaultCellStyle.SelectionForeColor = TextColor;
            dgv.DefaultCellStyle.Padding = new Padding(5);
            dgv.RowTemplate.Height = 30;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = SecondaryColor;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextColor;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        protected void StyleFormHeader(Label lbl)
        {
            lbl.Font = new Font("Segoe UI Light", 16F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            lbl.ForeColor = PrimaryColor;
            lbl.Padding = new Padding(0, 0, 0, 10);
        }
    }
}
