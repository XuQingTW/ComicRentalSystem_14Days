using System;
using System.IO;

namespace ComicRentalSystem_14Days.Helpers
{
    public static class DatabaseConfig
    {
        private const string DbName = "comic_rental.db";
        private const string AppFolderName = "ComicRentalApp"; 

        public static string GetConnectionString()
        {
            var customPath = Environment.GetEnvironmentVariable("COMIC_DB_PATH");
            if (!string.IsNullOrWhiteSpace(customPath))
            {
                var dir = Path.GetDirectoryName(customPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return $"Data Source={customPath}";
            }

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dbFolderPath = Path.Combine(appDataPath, AppFolderName);

            if (!Directory.Exists(dbFolderPath))
            {
                Directory.CreateDirectory(dbFolderPath);
            }

            string dbPath = Path.Combine(dbFolderPath, DbName);
            return $"Data Source={dbPath}";
        }
    }
}
