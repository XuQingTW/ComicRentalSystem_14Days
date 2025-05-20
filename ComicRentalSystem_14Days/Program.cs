// In Program.cs
using ComicRentalSystem_14Days.Interfaces; // 加入 using
using ComicRentalSystem_14Days.Logging;    // 加入 using

namespace ComicRentalSystem_14Days
{
    internal static class Program
    {
        public static ILogger? AppLogger { get; private set; } // 全局可訪問的 Logger 實例

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 初始化 Logger
            // 技術點4: 多型 (FileLogger 物件可以用 ILogger 型別的變數來引用)
            AppLogger = new FileLogger("ComicRentalSystemLog.txt");
            AppLogger.Log("Application starting..."); // 技術點3: 透過介面呼叫 Log()

            // 範例：處理未處理的例外 (可選，但建議)
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


            try
            {
                Application.Run(new MainForm(AppLogger)); // 將 Logger 傳遞給 MainForm
            }
            catch (Exception ex)
            {
                AppLogger?.LogError("Application terminated due to an unhandled exception in Main.", ex);
                // 可以選擇顯示一個錯誤訊息給使用者
                MessageBox.Show("應用程式發生嚴重錯誤，即將關閉。\n詳情請查看日誌檔案。", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                AppLogger?.Log("Application shutting down.");
            }
        }

        // 技術點5: 例外處理 (全域例外處理)
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            AppLogger?.LogError("Unhandled UI Thread Exception", e.Exception);
            // 可以顯示錯誤訊息
            MessageBox.Show($"發生未預期的UI執行緒錯誤: {e.Exception.Message}\n詳情請查看日誌。", "UI 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // 技術點5: 例外處理 (全域例外處理)
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppLogger?.LogError("Unhandled Non-UI Thread Exception", e.ExceptionObject as Exception);
            // 可以顯示錯誤訊息 (如果是在主執行緒或有UI互動的情況下)
            // 注意：非UI執行緒的例外直接用MessageBox可能會有問題
            if (e.IsTerminating)
            {
                // 記錄程式即將終止
                AppLogger?.Log("Application is terminating due to an unhandled exception.");
            }
        }
    }
}