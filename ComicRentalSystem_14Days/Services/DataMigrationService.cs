using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Logging;
using ComicRentalSystem_14Days.Models;
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
            _logger.Log("Initializing Database Context for migration...");
            _dbContext.Database.EnsureCreated();
            _logger.Log("Database schema ensured.");

            if (_dbContext.Comics.Any())
            {
                _logger.Log("Comics table is not empty. Assuming data migration has already occurred. Skipping.");
                return;
            }

            _logger.Log("No existing data found in Comics table. Proceeding with data migration.");

            ImportComics();
            ImportMembers();
            ImportUsers();

            if (_dbContext.ChangeTracker.HasChanges())
            {
                _logger.Log("Saving changes to SQLite database...");
                _dbContext.SaveChanges();
                _logger.Log("Data migration completed.");
            }
            else
            {
                _logger.Log("No data to migrate or no changes detected.");
            }
        }

        private void ImportComics()
        {
            string comicsCsvPath = _fileHelper.GetFullFilePath("comics.csv");
            if (!File.Exists(comicsCsvPath))
            {
                _logger.LogWarning($"Comics CSV file not found at {comicsCsvPath}. Skipping comic migration.");
                return;
            }

            _logger.Log($"Migrating comics from {comicsCsvPath}");
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
                        _logger.LogWarning($"Skipping malformed comic CSV line (not enough values): {line}");
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
                    _logger.LogError($"Error parsing comic CSV line '{line}': {ex.Message}", ex);
                }
            }
            _dbContext.Comics.AddRange(comicsToMigrate);
            _logger.Log($"Added {comicsToMigrate.Count} comics to context for migration.");
        }

        private void ImportMembers()
        {
            string membersCsvPath = _fileHelper.GetFullFilePath("members.csv");
            if (!File.Exists(membersCsvPath))
            {
                _logger.LogWarning($"Members CSV file not found at {membersCsvPath}. Skipping member migration.");
                return;
            }

            _logger.Log($"Migrating members from {membersCsvPath}");
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
                        _logger.LogWarning($"Skipping malformed member CSV line (not enough values): {line}");
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
                    _logger.LogError($"Error parsing member CSV line '{line}': {ex.Message}", ex);
                }
            }
            _dbContext.Members.AddRange(membersToMigrate);
            _logger.Log($"Added {membersToMigrate.Count} members to context for migration.");
        }

        private void ImportUsers()
        {
            string usersJsonPath = _fileHelper.GetFullFilePath("users.json");
            if (!File.Exists(usersJsonPath))
            {
                _logger.LogWarning($"Users JSON file not found at {usersJsonPath}. Skipping user migration.");
                return;
            }

            _logger.Log($"Migrating users from {usersJsonPath}");
            try
            {
                string usersJsonData = File.ReadAllText(usersJsonPath);
                if (!string.IsNullOrWhiteSpace(usersJsonData))
                {
                    var usersToMigrate = JsonSerializer.Deserialize<List<User>>(usersJsonData);
                    if (usersToMigrate != null && usersToMigrate.Any())
                    {
                        _dbContext.Users.AddRange(usersToMigrate);
                        _logger.Log($"Added {usersToMigrate.Count} users to context for migration.");
                    }
                    else
                    {
                        _logger.Log("User JSON was empty or deserialized to null/empty list.");
                    }
                }
                else
                {
                    _logger.Log("User JSON file is empty. Skipping user migration from JSON.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading or deserializing users JSON '{usersJsonPath}': {ex.Message}", ex);
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
