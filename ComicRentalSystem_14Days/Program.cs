using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Services;

namespace ComicRentalSystem_14Days
{
    internal static class Program
    {
        public static ILogger? AppLogger { get; private set; }
        public static FileHelper? AppFileHelper { get; private set; }
        public static ComicService? AppComicService { get; private set; }
        public static MemberService? AppMemberService { get; private set; }
        public static IReloadService? AppReloadService { get; private set; }


        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            AppLogger = new FileLogger("ComicRentalSystemLog.txt");
            AppLogger.Log("Application starting...");

            AppFileHelper = new FileHelper();
            AppReloadService = new ReloadService();

            if (AppFileHelper != null && AppLogger != null)
            {
                AppComicService = new ComicService(AppFileHelper, AppLogger);
                AppMemberService = new MemberService(AppFileHelper, AppLogger);
            }

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                // Ensure all critical services for MainForm are initialized
                if (AppLogger != null && AppComicService != null && AppMemberService != null && AppReloadService != null)
                {
                    Application.Run(new MainForm(AppLogger, AppComicService, AppMemberService, AppReloadService));
                }
                else
                {
                    // Adjusted error message to reflect that any of these services could be the cause
                    string missingServices = "";
                    if (AppLogger == null) missingServices += "Logger, ";
                    if (AppComicService == null) missingServices += "ComicService, ";
                    if (AppMemberService == null) missingServices += "MemberService, ";
                    if (AppReloadService == null) missingServices += "ReloadService, ";
                    if (!string.IsNullOrEmpty(missingServices))
                    {
                        missingServices = missingServices.Substring(0, missingServices.Length - 2); // Remove trailing comma and space
                    }

                    MessageBox.Show($"無法初始化核心服務 ({missingServices})，應用程式即將關閉。", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLogger?.LogError($"Application terminated because core services ({missingServices}) failed to initialize.");
                }
            }
            catch (Exception ex)
            {
                AppLogger?.LogError("Application terminated due to an unhandled exception in Main.", ex);
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
            MessageBox.Show($"發生未預期的UI執行緒錯誤: {e.Exception.Message}\n詳情請查看日誌。", "UI 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // 技術點5: 例外處理 (全域例外處理)
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppLogger?.LogError("Unhandled Non-UI Thread Exception", e.ExceptionObject as Exception);
            if (e.IsTerminating)
            {
                AppLogger?.Log("Application is terminating due to an unhandled exception.");
            }
        }
    }
}