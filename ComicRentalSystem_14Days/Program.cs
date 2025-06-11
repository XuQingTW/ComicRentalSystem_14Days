// 請將此段完整程式碼複製並取代您現有的 Program.cs 檔案

using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Services;
using System;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. 初始化應用程式設定
            ApplicationConfiguration.Initialize();

            // 2. 手動建立所有需要的服務
            ILogger logger = new FileLogger();
            IComicService comicService = new ComicService(logger);
            MemberService memberService = new MemberService(logger, comicService);
            AuthenticationService authService = new AuthenticationService(logger);
            IReloadService reloadService = new ReloadService(logger);

            // 3. 執行您原有的資料庫遷移和管理員檢查邏輯
            using (var dbContext = new Models.ComicRentalDbContext())
            {
                var migrationService = new DataMigrationService(logger, new FileHelper(), dbContext);
                migrationService.MigrateFromFiles();
            }
            authService.EnsureAdminUserExists("admin", "admin123");

            // 4. 設定全域例外處理
            Application.ThreadException += (s, e) =>
            {
                logger.LogError("未處理的UI執行緒例外狀況", e.Exception);
                MessageBox.Show($"發生未預期的UI執行緒錯誤: {e.Exception.Message}\n詳情請查看日誌。", "UI 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                logger.LogError("未處理的非UI執行緒例外狀況", e.ExceptionObject as Exception);
                if (e.IsTerminating)
                {
                    logger.Log("由於發生未處理的例外狀況，應用程式正在終止。");
                }
            };

            // 5. 建立並執行登入表單
            try
            {
                var loginForm = new LoginForm(logger, authService, comicService, memberService, reloadService);
                loginForm.Icon = IconHelper.GetAppIcon();
                Application.Run(loginForm);
            }
            catch (Exception ex)
            {
                logger.LogError("由於 Main 中發生未處理的例外狀況，應用程式已終止。", ex);
                MessageBox.Show("應用程式發生嚴重錯誤，即將關閉。\n詳情請查看日誌檔案。", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                logger.Log("應用程式關閉中。");
            }
        }
    }
}