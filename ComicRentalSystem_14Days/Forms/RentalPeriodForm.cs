using System;
using System.Windows.Forms;

using System.Drawing; // Color 所需

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RentalPeriodForm : BaseForm // 已變更繼承
    {
        public DateTime SelectedReturnDate { get; private set; }

        public RentalPeriodForm(DateTime minDate, DateTime maxDate)
        {
            InitializeComponent();

            // 套用現代樣式
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
                monthCalendarRental.ForeColor = ModernBaseForm.TextColor; // 影響日期數字
                monthCalendarRental.TitleBackColor = ModernBaseForm.PrimaryColor;
                monthCalendarRental.TitleForeColor = Color.White;
                monthCalendarRental.TrailingForeColor = Color.Gray; // 非當月日期
            }

            // 設定日期的原始邏輯
            monthCalendarRental.MinDate = minDate;
            monthCalendarRental.MaxDate = maxDate;

            // 嘗試將初始選定日期設定為 minDate (如果它在有效範圍內)。
            // 這也處理了如果未限制日期，minDate 可能晚于 monthCalendarRental.TodayDate 的情況。
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
                // 如果 TodayDate 也超出範圍，則為備用方案，但 MinDate 應為主要指南。
                monthCalendarRental.SelectionStart = minDate;
            }
            // 確保日曆顯示初始選取的日期。
             monthCalendarRental.SetDate(monthCalendarRental.SelectionStart);


            lblInfo.Text = $"請選擇歸還日期。\n最短租期: {minDate:yyyy-MM-dd}\n最長租期: {maxDate:yyyy-MM-dd}"; // "Select a return date.\nMin: {minDate:yyyy-MM-dd}\nMax: {maxDate:yyyy-MM-dd}"
        }

        private void btnConfirmRental_Click(object sender, EventArgs e)
        {
            // 確保選取的日期在日曆的有效最小/最大範圍內
            // (MonthCalendar 本身應強制執行此操作)。
            if (monthCalendarRental.SelectionStart >= monthCalendarRental.MinDate &&
                monthCalendarRental.SelectionStart <= monthCalendarRental.MaxDate)
            {
                this.SelectedReturnDate = monthCalendarRental.SelectionStart;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // 如果 MinDate 和 MaxDate 設定正確，理想情況下不應執行此案例。
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
