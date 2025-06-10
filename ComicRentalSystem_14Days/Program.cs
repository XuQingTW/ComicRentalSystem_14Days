using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILogger, FileLogger>();
                    services.AddSingleton<FileHelper>();
                    services.AddDbContext<Models.ComicRentalDbContext>();
                    services.AddSingleton<IComicService, ComicService>();
                    services.AddSingleton<MemberService>();
                    services.AddSingleton<AuthenticationService>();
                    services.AddSingleton<IReloadService, ReloadService>();
                    services.AddTransient<DataMigrationService>();
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger>();
            var authService = host.Services.GetRequiredService<AuthenticationService>();

            logger.Log("應用程式啟動中...");

            using (var scope = host.Services.CreateScope())
            {
                var migrationService = scope.ServiceProvider.GetRequiredService<DataMigrationService>();
                migrationService.MigrateFromFiles();
            }
            logger.Log("遷移檢查與處理完成後，已釋放資料庫內容。");

            authService.EnsureAdminUserExists("admin", "admin123");

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

            try
            {
                var loginForm = ActivatorUtilities.CreateInstance<LoginForm>(host.Services);
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
