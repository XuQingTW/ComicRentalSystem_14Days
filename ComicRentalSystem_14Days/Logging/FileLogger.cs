using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ComicRentalSystem_14Days.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly string _backupDirectory;
        private readonly string _settingsPath;
        private int _retentionDays = 90;
        private static readonly object _lock = new object();

        public FileLogger()
        {
            _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
            _backupDirectory = Path.Combine(_logDirectory, "Backup");
            _settingsPath = Path.Combine(_logDirectory, "logsettings.json");
            EnsureDirectories();
            LoadSettings();
            CleanUpOldLogs();
        }

        private void EnsureDirectories()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                if (!Directory.Exists(_backupDirectory))
                {
                    Directory.CreateDirectory(_backupDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[嚴重] 無法建立日誌目錄 '{_logDirectory}': {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(_settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<LogSettings>(json);
                    if (settings != null)
                    {
                        _retentionDays = settings.RetentionDays;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[日誌設定錯誤] 讀取設定檔失敗: {ex.Message}");
                }
            }
            else
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new LogSettings { RetentionDays = _retentionDays };
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(_settingsPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[日誌設定錯誤] 儲存設定檔失敗: {ex.Message}");
            }
        }

        public void SetRetentionDays(int days)
        {
            if (days <= 0) return;
            _retentionDays = days;
            SaveSettings();
            CleanUpOldLogs();
        }

        private string GetLogFilePathForDate(DateTime date)
        {
            string fileName = $"{date:yyyyMMdd}.txt";
            return Path.Combine(_logDirectory, fileName);
        }

        public void DeleteLogByDate(DateTime date)
        {
            string path = GetLogFilePathForDate(date);
            if (File.Exists(path))
            {
                BackupFile(path);
                File.Delete(path);
            }
        }

        private void BackupFile(string path)
        {
            try
            {
                string dest = Path.Combine(_backupDirectory, Path.GetFileName(path));
                File.Copy(path, dest, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[日誌備份錯誤] {ex.Message}");
            }
        }

        private void CleanUpOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(_logDirectory, "*.txt");
                foreach (var file in files)
                {
                    var creation = File.GetCreationTime(file);
                    if ((DateTime.Now - creation).TotalDays > _retentionDays)
                    {
                        BackupFile(file);
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[日誌清理錯誤] {ex.Message}");
            }
        }

        public void Log(string message)
        {
            WriteLogEntry("資訊", message);
        }

        public void Log(string message, Exception ex)
        {
            WriteLogEntry("錯誤", $"{message} - 例外: {ex}");
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                WriteLogEntry("錯誤", $"{message} - 例外: {ex}");
            }
            else
            {
                WriteLogEntry("錯誤", message);
            }
        }

        public void LogWarning(string message)
        {
            WriteLogEntry("警告", message);
        }

        public void LogInformation(string message)
        {
            WriteLogEntry("資訊", message);
        }

        public void LogDebug(string message)
        {
            WriteLogEntry("偵錯", message);
        }

        private void WriteLogEntry(string level, string message)
        {
            try
            {
                string path = GetLogFilePathForDate(DateTime.Now);
                lock (_lock)
                {
                    File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] - {message}{Environment.NewLine}", Encoding.UTF8);
                }
                CleanUpOldLogs();
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[檔案記錄器 IO 錯誤] 無法寫入日誌檔案: {ioEx.Message}。原始訊息: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[檔案記錄器 未預期錯誤] 無法寫入日誌檔案: {ex.Message}。原始訊息: {message}");
            }
        }
    }
}
