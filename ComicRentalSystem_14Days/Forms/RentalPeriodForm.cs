using System;
using System.Windows.Forms;

using System.Drawing; 

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RentalPeriodForm : BaseForm 
    {
        public DateTime SelectedReturnDate { get; private set; }

        public RentalPeriodForm(DateTime minDate, DateTime maxDate)
        {
            InitializeComponent();

            if (btnConfirmRental != null) StyleModernButton(btnConfirmRental);
            if (btnCancelRental != null) StyleSecondaryButton(btnCancelRental);

            if (lblInfo != null)
            {
                lblInfo.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
                lblInfo.ForeColor = ModernBaseForm.TextColor;
            }

            if (monthCalendarRental != null)
            {
                monthCalendarRental.BackColor = ModernBaseForm.SecondaryColor;
                monthCalendarRental.ForeColor = ModernBaseForm.TextColor; 
                monthCalendarRental.TitleBackColor = ModernBaseForm.PrimaryColor;
                monthCalendarRental.TitleForeColor = Color.White;
                monthCalendarRental.TrailingForeColor = Color.Gray; 
            }

            monthCalendarRental.MinDate = minDate;
            monthCalendarRental.MaxDate = maxDate;

            if (minDate > monthCalendarRental.TodayDate && minDate <= maxDate)
            {
                monthCalendarRental.SelectionStart = minDate;
            }
            else if (monthCalendarRental.TodayDate >= minDate && monthCalendarRental.TodayDate <= maxDate)
            {
                 monthCalendarRental.SelectionStart = monthCalendarRental.TodayDate;
            }
            else
            {
                monthCalendarRental.SelectionStart = minDate;
            }
             monthCalendarRental.SetDate(monthCalendarRental.SelectionStart);


            lblInfo.Text = $"請選擇歸還日期。\n最短租期: {minDate:yyyy-MM-dd}\n最長租期: {maxDate:yyyy-MM-dd}"; // "Select a return date.\nMin: {minDate:yyyy-MM-dd}\nMax: {maxDate:yyyy-MM-dd}"
        }

        private void btnConfirmRental_Click(object sender, EventArgs e)
        {
            if (monthCalendarRental.SelectionStart >= monthCalendarRental.MinDate &&
                monthCalendarRental.SelectionStart <= monthCalendarRental.MaxDate)
            {
                this.SelectedReturnDate = monthCalendarRental.SelectionStart;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("請選擇有效範圍內的日期。", "日期無效", MessageBoxButtons.OK, MessageBoxIcon.Warning); // "Please select a valid date within the allowed range." | "Invalid Date"
            }
        }

        private void btnCancelRental_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
