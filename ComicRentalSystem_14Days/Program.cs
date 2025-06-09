using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Models; // Added for DbContext and Models
using Microsoft.EntityFrameworkCore;    // Added for EF Core operations
using System.Text.Json;                 // Added for User data deserialization
using System.Collections.Generic;       // Added for List<T>
using System.Linq;                      // Added for .Any() and other LINQ methods
using System.IO;                        // Added for File operations
using System.Text;                      // Added for StringBuilder

namespace ComicRentalSystem_14Days
{
    internal static class Program
    {
        public static ILogger? AppLogger { get; private set; }
        public static FileHelper? AppFileHelper { get; private set; } // Retained for now, in case FileLogger or other parts might use it.
        public static ComicRentalDbContext? AppDbContext { get; private set; } // Added DbContext
        public static IComicService? AppComicService { get; private set; }
        public static MemberService? AppMemberService { get; private set; }
        public static IReloadService? AppReloadService { get; private set; }
        public static AuthenticationService? AppAuthService { get; private set; }


        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            AppLogger = new FileLogger();
            AppLogger.Log("應用程式啟動中...");

            AppFileHelper = new FileHelper();
            AppReloadService = new ReloadService();

            // --- BEGIN DATA MIGRATION TO SQLITE ---
            AppLogger.Log("Initializing Database Context for migration...");
            using (var migrationContext = new ComicRentalDbContext())
            {
                AppLogger.Log("Ensuring database is created...");
                migrationContext.Database.EnsureCreated(); // Creates DB and schema if not exists
                AppLogger.Log("Database schema ensured.");

                // Check if migration has already run (e.g., by checking if Comics table has data)
                if (!migrationContext.Comics.Any())
                {
                    AppLogger.Log("No existing data found in Comics table. Proceeding with data migration.");

                    // --- Migrate Comics ---
                    string comicsCsvPath = AppFileHelper.GetFullFilePath("comics.csv");
                    if (File.Exists(comicsCsvPath))
                    {
                        AppLogger.Log($"Migrating comics from {comicsCsvPath}");
                        var comicLines = File.ReadAllLines(comicsCsvPath);
                        var comicsToMigrate = new List<Comic>();
                        foreach (var line in comicLines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                // Embedded Comic.FromCsvString logic (simplified)
                                List<string> values = ParseCsvLineInternal(line);
                                if (values.Count < 7) { AppLogger.LogWarning($"Skipping malformed comic CSV line (not enough values): {line}"); continue; }
                                var comic = new Comic
                                {
                                    Id = int.Parse(values[0]),
                                    Title = values[1],
                                    Author = values[2],
                                    Isbn = values[3],
                                    Genre = values[4],
                                    IsRented = bool.Parse(values[5]),
                                    RentedToMemberId = string.IsNullOrEmpty(values[6]) ? 0 : int.Parse(values[6])
                                };
                                if (values.Count > 7 && !string.IsNullOrEmpty(values[7]) && DateTime.TryParse(values[7], out DateTime rd)) comic.RentalDate = rd;
                                if (values.Count > 8 && !string.IsNullOrEmpty(values[8]) && DateTime.TryParse(values[8], out DateTime retd)) comic.ReturnDate = retd;
                                if (values.Count > 9 && !string.IsNullOrEmpty(values[9]) && DateTime.TryParse(values[9], out DateTime art)) comic.ActualReturnTime = art;
                                comicsToMigrate.Add(comic);
                            }
                            catch (Exception ex) { AppLogger.LogError($"Error parsing comic CSV line '{line}': {ex.Message}", ex); }
                        }
                        migrationContext.Comics.AddRange(comicsToMigrate);
                        AppLogger.Log($"Added {comicsToMigrate.Count} comics to context for migration.");
                    } else { AppLogger.LogWarning($"Comics CSV file not found at {comicsCsvPath}. Skipping comic migration.");}

                    // --- Migrate Members ---
                    string membersCsvPath = AppFileHelper.GetFullFilePath("members.csv");
                    if (File.Exists(membersCsvPath))
                    {
                        AppLogger.Log($"Migrating members from {membersCsvPath}");
                        var memberLines = File.ReadAllLines(membersCsvPath);
                        var membersToMigrate = new List<Member>();
                        foreach (var line in memberLines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                // Embedded Member.FromCsvString logic (simplified)
                                List<string> values = ParseCsvLineInternal(line);
                                if (values.Count < 3) { AppLogger.LogWarning($"Skipping malformed member CSV line (not enough values): {line}"); continue; }
                                var member = new Member
                                {
                                    Id = int.Parse(values[0]),
                                    Name = values[1],
                                    PhoneNumber = values[2],
                                    Username = values.Count > 3 ? values[3] : values[1] // Default Username to Name if not present
                                };
                                membersToMigrate.Add(member);
                            }
                            catch (Exception ex) { AppLogger.LogError($"Error parsing member CSV line '{line}': {ex.Message}", ex); }
                        }
                        migrationContext.Members.AddRange(membersToMigrate);
                        AppLogger.Log($"Added {membersToMigrate.Count} members to context for migration.");
                    } else { AppLogger.LogWarning($"Members CSV file not found at {membersCsvPath}. Skipping member migration.");}

                    // --- Migrate Users (from users.json) ---
                    string usersJsonPath = AppFileHelper.GetFullFilePath("users.json");
                    if (File.Exists(usersJsonPath))
                    {
                        AppLogger.Log($"Migrating users from {usersJsonPath}");
                        try
                        {
                            string usersJsonData = File.ReadAllText(usersJsonPath);
                            if (!string.IsNullOrWhiteSpace(usersJsonData))
                            {
                                var usersToMigrate = JsonSerializer.Deserialize<List<User>>(usersJsonData);
                                if (usersToMigrate != null && usersToMigrate.Any())
                                {
                                    migrationContext.Users.AddRange(usersToMigrate);
                                    AppLogger.Log($"Added {usersToMigrate.Count} users to context for migration.");
                                } else { AppLogger.Log("User JSON was empty or deserialized to null/empty list."); }
                            } else { AppLogger.Log("User JSON file is empty. Skipping user migration from JSON."); }
                        }
                        catch (Exception ex) { AppLogger.LogError($"Error reading or deserializing users JSON '{usersJsonPath}': {ex.Message}", ex); }
                    } else { AppLogger.LogWarning($"Users JSON file not found at {usersJsonPath}. Skipping user migration.");}

                    if (migrationContext.ChangeTracker.HasChanges())
                    {
                        AppLogger.Log("Saving changes to SQLite database...");
                        migrationContext.SaveChanges();
                        AppLogger.Log("Data migration completed.");
                    }
                    else
                    {
                        AppLogger.Log("No data to migrate or no changes detected.");
                    }
                }
                else
                {
                    AppLogger.Log("Comics table is not empty. Assuming data migration has already occurred. Skipping.");
                }
            }
            AppLogger.Log("Database context disposed after migration check/process.");
            // --- END DATA MIGRATION TO SQLITE ---

            AppLogger.Log("Initializing main application DbContext...");
            AppDbContext = new ComicRentalDbContext();
            AppLogger.Log("Main application DbContext initialized.");

            if (AppLogger != null)
            {
                AppComicService = new ComicService(AppLogger);
                AppLogger.Log("ComicService initialized.");

                AppMemberService = new MemberService(AppLogger, AppComicService);
                AppLogger.Log("MemberService initialized.");

                AppAuthService = new AuthenticationService(AppLogger);
                AppLogger.Log("AuthenticationService initialized.");

                AppAuthService.EnsureAdminUserExists("admin", "admin123");
            }
            else
            {
                AppLogger?.LogError("Critical error: AppLogger is null. Cannot initialize core services.");
            }

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                if (AppLogger != null && AppAuthService != null && AppComicService != null && AppMemberService != null && AppReloadService != null)
                {
                    Application.Run(new LoginForm(AppLogger, AppAuthService, AppComicService, AppMemberService, AppReloadService));
                }
                else
                {
                    MessageBox.Show("無法初始化核心服務，應用程式即將關閉。", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLogger?.LogError("由於核心服務 (Logger、AuthService、ComicService、MemberService 或 ReloadService) 初始化失敗，應用程式已終止。");
                }
            }
            catch (Exception ex)
            {
                AppLogger?.LogError("由於 Main 中發生未處理的例外狀況，應用程式已終止。", ex);
                MessageBox.Show("應用程式發生嚴重錯誤，即將關閉。\n詳情請查看日誌檔案。", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                AppLogger?.Log("應用程式關閉中。");
            }
        }

        private static List<string> ParseCsvLineInternal(string csvLine)
        {
            // This is the same robust parser from Comic.cs / Member.cs
            // (Copied here to avoid making models dirty again or creating temp classes for this one-time migration)
            List<string> fields = new List<string>();
            StringBuilder fieldBuilder = new StringBuilder();
            bool inQuotes = false;
            bool currentFieldWasQuoted = false;
            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                        {
                            fieldBuilder.Append('"'); i++;
                        }
                        else { inQuotes = false; }
                    }
                    else { fieldBuilder.Append(c); }
                }
                else
                {
                    if (c == '"')
                    {
                        if (fieldBuilder.Length == 0) { inQuotes = true; currentFieldWasQuoted = true; }
                        else { fieldBuilder.Append(c); }
                    }
                    else if (c == ',')
                    {
                        fields.Add(currentFieldWasQuoted ? fieldBuilder.ToString() : fieldBuilder.ToString().Trim());
                        fieldBuilder.Clear(); currentFieldWasQuoted = false;
                    }
                    else { fieldBuilder.Append(c); }
                }
            }
            fields.Add(currentFieldWasQuoted ? fieldBuilder.ToString() : fieldBuilder.ToString().Trim());
            return fields;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            AppLogger?.LogError("未處理的UI執行緒例外狀況", e.Exception);
            MessageBox.Show($"發生未預期的UI執行緒錯誤: {e.Exception.Message}\n詳情請查看日誌。", "UI 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppLogger?.LogError("未處理的非UI執行緒例外狀況", e.ExceptionObject as Exception);
            if (e.IsTerminating)
            {
                AppLogger?.Log("由於發生未處理的例外狀況，應用程式正在終止。");
            }
        }
    }
}