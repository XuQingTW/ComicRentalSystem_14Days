using System;
using ComicRentalSystem_14Days.Interfaces;
using System.Windows.Forms;
using System.Drawing; 
using System.ComponentModel; 

namespace ComicRentalSystem_14Days
{
    [DesignerCategory("Form")]
    public class BaseForm : Form
    {
        protected ILogger? Logger { get; private set; }

        protected BaseForm()
        {
            InitializeBaseFormProperties();
        }

        protected BaseForm(ILogger logger) : this()
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void InitializeBaseFormProperties()
        {
            // 保持註解，除非確定它們不是問題
            // this.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            // this.StartPosition = FormStartPosition.CenterScreen;
            // this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // this.MaximizeBox = false;
        }

        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogActivity(string message)
        {
            if (Logger != null)
            {
                Logger.Log($"[{this.Name} ACTIVITY]: {message}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[LOG - {this.Name} - NO LOGGER]: {message} at {DateTime.Now}");
            }
        }

        protected void LogErrorActivity(string message, Exception? ex = null)
        {
            if (Logger != null)
            {
                Logger.LogError($"[{this.Name} ERROR]: {message}", ex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR LOG - {this.Name} - NO LOGGER]: {message} {(ex != null ? ex.ToString() : "")} at {DateTime.Now}");
            }
        }
    }
}