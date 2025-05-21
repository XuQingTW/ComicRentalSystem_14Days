// In ComicRentalSystem_14Days/BaseForm.cs
using System;
using ComicRentalSystem_14Days.Interfaces; // 確保這個 using 存在
using System.Windows.Forms;
using System.Drawing; // For Font
using System.ComponentModel; // For DesignerCategory

namespace ComicRentalSystem_14Days
{
    [DesignerCategory("Form")] // 保留此屬性
    public class BaseForm : Form // 恢復 abstract
    {
        protected ILogger? Logger { get; private set; }

        // 無參數建構函式，供設計工具和衍生類別使用
        protected BaseForm()
        {
            InitializeBaseFormProperties();
        }

        // 帶 ILogger 參數的建構函式，供運行時使用
        protected BaseForm(ILogger logger) : this() // 呼叫無參數建構函式以執行 InitializeBaseFormProperties
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void InitializeBaseFormProperties()
        {
            // 保持這些註解，除非您確定它們不是問題
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