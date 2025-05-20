using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicRentalSystem_14Days.Interfaces;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    public abstract class BaseForm : Form // 建議將 BaseForm 設為 abstract 如果它不應該被直接實例化
    {
        protected ILogger? Logger { get; private set; } // 讓子類可以訪問

        // 修改建構函式以接收 ILogger (可選，如果子類會直接傳入)
        // 或者提供一個方法來設定 Logger
        protected BaseForm() // 預設建構函式
        {
            InitializeBaseFormProperties();
        }

        // 過載建構函式，接收 ILogger (技術點4: 過載)
        protected BaseForm(ILogger logger)
        {
            InitializeBaseFormProperties();
            Logger = logger;
        }

        private void InitializeBaseFormProperties()
        {
            this.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }


        // 讓子表單可以設定 Logger，如果它們不是透過建構函式接收的話
        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 修改 LogActivity，使用 ILogger
        // 技術點3: 介面 (ILogger)
        // 技術點4: 多型 (透過 ILogger 介面呼叫 Log())
        protected void LogActivity(string message)
        {
            // Logger?.Log($"[{this.Name}]: {message}"); // 原本直接呼叫Console.WriteLine
            // 現在透過 ILogger 介面
            if (Logger != null)
            {
                // 技術點4: 多型 (實際呼叫的是 FileLogger 的 Log 方法)
                Logger.Log($"[{this.Name} ACTIVITY]: {message}");
            }
            else
            {
                // 如果 Logger 未設定，可以提供一個備案，例如輸出到主控台
                Console.WriteLine($"[LOG - {this.Name} - NO LOGGER]: {message} at {DateTime.Now}");
            }
        }

        protected void LogErrorActivity(string message, Exception? ex = null)
        {
            if (Logger != null)
            {
                // 技術點4: 多型 (實際呼叫的是 FileLogger 的 LogError 方法)
                // 技術點4: 過載 (ILogger 的 LogError 方法)
                Logger.LogError($"[{this.Name} ERROR]: {message}", ex);
            }
            else
            {
                Console.WriteLine($"[ERROR LOG - {this.Name} - NO LOGGER]: {message} {(ex != null ? ex.ToString() : "")} at {DateTime.Now}");
            }
        }
    }
}