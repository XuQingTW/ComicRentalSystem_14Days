using System;
using System.IO;

namespace ComicRentalSystem_14Days.Helpers
{
    public static class DatabaseConfig
    {
        private const string DbName = "comic_rental.db";
        private const string AppFolderName = "ComicRentalApp"; // Same as in FileHelper for consistency

        public static string GetConnectionString()
        {
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
