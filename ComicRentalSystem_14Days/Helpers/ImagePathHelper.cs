using System;
using System.IO;

namespace ComicRentalSystem_14Days.Helpers
{
    public static class ImagePathHelper
    {
        private static readonly string CoverFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoverImages");

        static ImagePathHelper()
        {
            if (!Directory.Exists(CoverFolder))
            {
                Directory.CreateDirectory(CoverFolder);
            }
        }

        public static string SaveToAppFolder(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath)) return string.Empty;

            string extension = Path.GetExtension(sourcePath);
            string destFileName = $"{Guid.NewGuid()}{extension}";
            string destFullPath = Path.Combine(CoverFolder, destFileName);
            File.Copy(sourcePath, destFullPath, true);
            return Path.Combine("CoverImages", destFileName);
        }

        public static string GetFullPath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
            if (Path.IsPathRooted(relativePath)) return relativePath;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }
    }
}
