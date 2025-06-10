using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ComicRentalSystem_14Days.Services
{
    public class DataMigrationService
    {
        private readonly ILogger _logger;
        private readonly FileHelper _fileHelper;
        private readonly ComicRentalDbContext _dbContext;

        public DataMigrationService(ILogger logger, FileHelper fileHelper, ComicRentalDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void MigrateFromFiles()
        {
            _logger.Log("正在初始化資料庫內容以進行資料移轉...");
            _dbContext.Database.EnsureCreated();
            _logger.Log("已確認資料庫結構。");

            if (_dbContext.Comics.Any())
            {
                _logger.Log("Comics 資料表不為空，推斷資料已經移轉過，略過此步驟。");
                return;
            }

            _logger.Log("在 Comics 資料表未找到既有資料，開始進行資料移轉。");

            ImportComics();
            ImportMembers();
            ImportUsers();

            if (_dbContext.ChangeTracker.HasChanges())
            {
                _logger.Log("正在將變更儲存至 SQLite 資料庫...");
                _dbContext.SaveChanges();
                _logger.Log("資料移轉完成。");
            }
            else
            {
                _logger.Log("無資料可移轉或未偵測到任何變更。");
            }
        }

        private void ImportComics()
        {
            string comicsCsvPath = _fileHelper.GetFullFilePath("comics.csv");
            if (!File.Exists(comicsCsvPath))
            {
                _logger.LogWarning($"找不到漫畫 CSV 檔 {comicsCsvPath}，略過漫畫資料移轉。");
                return;
            }

            _logger.Log($"從 {comicsCsvPath} 匯入漫畫資料");
            var comicLines = File.ReadAllLines(comicsCsvPath);
            var comicsToMigrate = new List<Comic>();
            foreach (var line in comicLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    List<string> values = ParseCsvLineInternal(line);
                    if (values.Count < 7)
                    {
                        _logger.LogWarning($"略過格式不正確的漫畫 CSV 行：{line} (欄位數不足)");
                        continue;
                    }
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
                catch (Exception ex)
                {
                    _logger.LogError($"解析漫畫 CSV 行 '{line}' 時發生錯誤：{ex.Message}", ex);
                }
            }
            _dbContext.Comics.AddRange(comicsToMigrate);
            _logger.Log($"已將 {comicsToMigrate.Count} 本漫畫加入移轉內容。");
        }

        private void ImportMembers()
        {
            string membersCsvPath = _fileHelper.GetFullFilePath("members.csv");
            if (!File.Exists(membersCsvPath))
            {
                _logger.LogWarning($"找不到會員 CSV 檔 {membersCsvPath}，略過會員資料移轉。");
                return;
            }

            _logger.Log($"從 {membersCsvPath} 匯入會員資料");
            var memberLines = File.ReadAllLines(membersCsvPath);
            var membersToMigrate = new List<Member>();
            foreach (var line in memberLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    List<string> values = ParseCsvLineInternal(line);
                    if (values.Count < 3)
                    {
                        _logger.LogWarning($"略過格式不正確的會員 CSV 行：{line} (欄位數不足)");
                        continue;
                    }
                    var member = new Member
                    {
                        Id = int.Parse(values[0]),
                        Name = values[1],
                        PhoneNumber = values[2],
                        Username = values.Count > 3 ? values[3] : values[1]
                    };
                    membersToMigrate.Add(member);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"解析會員 CSV 行 '{line}' 時發生錯誤：{ex.Message}", ex);
                }
            }
            _dbContext.Members.AddRange(membersToMigrate);
            _logger.Log($"已將 {membersToMigrate.Count} 位會員加入移轉內容。");
        }

        private void ImportUsers()
        {
            string usersJsonPath = _fileHelper.GetFullFilePath("users.json");
            if (!File.Exists(usersJsonPath))
            {
                _logger.LogWarning($"找不到使用者 JSON 檔 {usersJsonPath}，略過使用者資料移轉。");
                return;
            }

            _logger.Log($"從 {usersJsonPath} 匯入使用者資料");
            try
            {
                string usersJsonData = File.ReadAllText(usersJsonPath);
                if (!string.IsNullOrWhiteSpace(usersJsonData))
                {
                    var usersToMigrate = JsonSerializer.Deserialize<List<User>>(usersJsonData);
                    if (usersToMigrate != null && usersToMigrate.Any())
                    {
                        _dbContext.Users.AddRange(usersToMigrate);
                        _logger.Log($"已將 {usersToMigrate.Count} 位使用者加入移轉內容。");
                    }
                    else
                    {
                        _logger.Log("使用者 JSON 為空或反序列化後為空清單。");
                    }
                }
                else
                {
                    _logger.Log("使用者 JSON 檔為空，略過從 JSON 移轉使用者。");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"讀取或反序列化使用者 JSON '{usersJsonPath}' 時發生錯誤：{ex.Message}", ex);
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
