using System;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RentalPeriodForm : Form
    {
        public DateTime SelectedReturnDate { get; private set; }

        public RentalPeriodForm(DateTime minDate, DateTime maxDate)
        {
            InitializeComponent();

            monthCalendarRental.MinDate = minDate;
            monthCalendarRental.MaxDate = maxDate;

            // Attempt to set the initial selected date to minDate, if it's within the valid range.
            // This also handles the case where minDate might be later than monthCalendarRental.TodayDate if not clamped.
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
                // Fallback if TodayDate is also out of range, though MinDate should be the primary guide.
                monthCalendarRental.SelectionStart = minDate;
            }
            // Ensure the calendar shows the initial selection.
             monthCalendarRental.SetDate(monthCalendarRental.SelectionStart);


            lblInfo.Text = $"Select a return date.\nMin: {minDate:yyyy-MM-dd}\nMax: {maxDate:yyyy-MM-dd}";
        }

        private void btnConfirmRental_Click(object sender, EventArgs e)
        {
            // Ensure a date is selected and it's within the calendar's effective min/max range
            // (which should be enforced by MonthCalendar itself).
            if (monthCalendarRental.SelectionStart >= monthCalendarRental.MinDate &&
                monthCalendarRental.SelectionStart <= monthCalendarRental.MaxDate)
            {
                this.SelectedReturnDate = monthCalendarRental.SelectionStart;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // This case should ideally not be hit if MinDate and MaxDate are set correctly.
                MessageBox.Show("Please select a valid date within the allowed range.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelRental_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
