using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
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
                    services.AddDbContext<ComicRentalDbContext>();
                    services.AddSingleton<IComicService, ComicService>();
                    services.AddSingleton<MemberService>();
                    services.AddSingleton<AuthenticationService>();
                    services.AddSingleton<IReloadService, ReloadService>();
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger>();
            var fileHelper = host.Services.GetRequiredService<FileHelper>();
            var authService = host.Services.GetRequiredService<AuthenticationService>();

            logger.Log("應用程式啟動中...");

            logger.Log("Initializing Database Context for migration...");
            using (var scope = host.Services.CreateScope())
            {
                var migrationContext = scope.ServiceProvider.GetRequiredService<ComicRentalDbContext>();
                logger.Log("Ensuring database is created...");
                migrationContext.Database.EnsureCreated();
                logger.Log("Database schema ensured.");

                if (!migrationContext.Comics.Any())
                {
                    logger.Log("No existing data found in Comics table. Proceeding with data migration.");
                    string comicsCsvPath = fileHelper.GetFullFilePath("comics.csv");
                    if (File.Exists(comicsCsvPath))
                    {
                        logger.Log($"Migrating comics from {comicsCsvPath}");
                        var comicLines = File.ReadAllLines(comicsCsvPath);
                        var comicsToMigrate = new List<Comic>();
                        foreach (var line in comicLines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                List<string> values = ParseCsvLineInternal(line);
                                if (values.Count < 7) { logger.LogWarning($"Skipping malformed comic CSV line (not enough values): {line}"); continue; }
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
                            catch (Exception ex) { logger.LogError($"Error parsing comic CSV line '{line}': {ex.Message}", ex); }
                        }
                        migrationContext.Comics.AddRange(comicsToMigrate);
                        logger.Log($"Added {comicsToMigrate.Count} comics to context for migration.");
                    }
                    else
                    {
                        logger.LogWarning($"Comics CSV file not found at {comicsCsvPath}. Skipping comic migration.");
                    }

                    string membersCsvPath = fileHelper.GetFullFilePath("members.csv");
                    if (File.Exists(membersCsvPath))
                    {
                        logger.Log($"Migrating members from {membersCsvPath}");
                        var memberLines = File.ReadAllLines(membersCsvPath);
                        var membersToMigrate = new List<Member>();
                        foreach (var line in memberLines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                List<string> values = ParseCsvLineInternal(line);
                                if (values.Count < 3) { logger.LogWarning($"Skipping malformed member CSV line (not enough values): {line}"); continue; }
                                var member = new Member
                                {
                                    Id = int.Parse(values[0]),
                                    Name = values[1],
                                    PhoneNumber = values[2],
                                    Username = values.Count > 3 ? values[3] : values[1]
                                };
                                membersToMigrate.Add(member);
                            }
                            catch (Exception ex) { logger.LogError($"Error parsing member CSV line '{line}': {ex.Message}", ex); }
                        }
                        migrationContext.Members.AddRange(membersToMigrate);
                        logger.Log($"Added {membersToMigrate.Count} members to context for migration.");
                    }
                    else
                    {
                        logger.LogWarning($"Members CSV file not found at {membersCsvPath}. Skipping member migration.");
                    }

                    string usersJsonPath = fileHelper.GetFullFilePath("users.json");
                    if (File.Exists(usersJsonPath))
                    {
                        logger.Log($"Migrating users from {usersJsonPath}");
                        try
                        {
                            string usersJsonData = File.ReadAllText(usersJsonPath);
                            if (!string.IsNullOrWhiteSpace(usersJsonData))
                            {
                                var usersToMigrate = JsonSerializer.Deserialize<List<User>>(usersJsonData);
                                if (usersToMigrate != null && usersToMigrate.Any())
                                {
                                    migrationContext.Users.AddRange(usersToMigrate);
                                    logger.Log($"Added {usersToMigrate.Count} users to context for migration.");
                                }
                                else
                                {
                                    logger.Log("User JSON was empty or deserialized to null/empty list.");
                                }
                            }
                            else
                            {
                                logger.Log("User JSON file is empty. Skipping user migration from JSON.");
                            }
                        }
                        catch (Exception ex) { logger.LogError($"Error reading or deserializing users JSON '{usersJsonPath}': {ex.Message}", ex); }
                    }
                    else
                    {
                        logger.LogWarning($"Users JSON file not found at {usersJsonPath}. Skipping user migration.");
                    }

                    if (migrationContext.ChangeTracker.HasChanges())
                    {
                        logger.Log("Saving changes to SQLite database...");
                        migrationContext.SaveChanges();
                        logger.Log("Data migration completed.");
                    }
                    else
                    {
                        logger.Log("No data to migrate or no changes detected.");
                    }
                }
                else
                {
                    logger.Log("Comics table is not empty. Assuming data migration has already occurred. Skipping.");
                }
            }
            logger.Log("Database context disposed after migration check/process.");

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

        private static List<string> ParseCsvLineInternal(string csvLine)
        {
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
                            fieldBuilder.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        if (fieldBuilder.Length == 0)
                        {
                            inQuotes = true;
                            currentFieldWasQuoted = true;
                        }
                        else
                        {
                            fieldBuilder.Append(c);
                        }
                    }
                    else if (c == ',')
                    {
                        fields.Add(currentFieldWasQuoted ? fieldBuilder.ToString() : fieldBuilder.ToString().Trim());
                        fieldBuilder.Clear();
                        currentFieldWasQuoted = false;
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }
            fields.Add(currentFieldWasQuoted ? fieldBuilder.ToString() : fieldBuilder.ToString().Trim());
            return fields;
        }
    }
}
