// BaseForm.cs

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Interfaces; // 必須引用 ILogger 所在命名空間

namespace ComicRentalSystem_14Days
{
    [DesignerCategory("Form")]
    public class BaseForm : ModernBaseForm
    {
        protected ILogger? Logger { get; private set; }

        // 無參建構式，直接呼叫 ModernBaseForm 的無參建構式
        protected BaseForm() : base()
        {
        }

        // 帶有 ILogger 的建構式，只呼叫同類別的無參建構式 (this())
        protected BaseForm(ILogger logger) : this()
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogActivity(string message)
        {
            if (Logger != null)
            {
                Logger.Log($"[{this.Name} 活動]: {message}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[日誌 - {this.Name} - 無記錄器]: {message} at {DateTime.Now}");
            }
        }

        protected void LogErrorActivity(string message, Exception? ex = null)
        {
            if (Logger != null)
            {
                Logger.LogError($"[{this.Name} 錯誤]: {message}", ex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[錯誤日誌 - {this.Name} - 無記錄器]: {message} {(ex != null ? ex.ToString() : "")} at {DateTime.Now}");
            }
        }
    }
}
