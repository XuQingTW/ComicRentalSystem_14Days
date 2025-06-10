using ComicRentalSystem_14Days.Helpers;
using System;
using System.IO;

namespace ComicRentalSystem.Tests
{
    [TestClass]
    public class DatabaseConfigTests
    {
        private string? _originalDbPath;
        private string _tempPath = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _originalDbPath = Environment.GetEnvironmentVariable("COMIC_DB_PATH");
            _tempPath = Path.Combine(Path.GetTempPath(), $"testdb_{Guid.NewGuid()}.db");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("COMIC_DB_PATH", _originalDbPath);
            if (File.Exists(_tempPath))
            {
                File.Delete(_tempPath);
            }
            var dir = Path.GetDirectoryName(_tempPath);
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0)
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void GetConnectionString_UsesEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("COMIC_DB_PATH", _tempPath);
            var conn = DatabaseConfig.GetConnectionString();
            Assert.AreEqual($"Data Source={_tempPath}", conn);
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(_tempPath)!));
        }
    }
}
