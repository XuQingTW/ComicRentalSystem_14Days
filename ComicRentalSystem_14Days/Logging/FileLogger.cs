using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly string _backupDirectory;
        private readonly string _settingsPath;
        private int _retentionDays = 90;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly List<Func<Task>> _logQueue = new List<Func<Task>>();
        private Task _loggingTask = Task.CompletedTask;
        private readonly object _queueLock = new object();

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

        // New method to process the queue
        private async Task ProcessLogQueueAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                // Swap and clear queue inside lock
                List<Func<Task>> currentTasks;
                lock (_queueLock)
                {
                    if (!_logQueue.Any()) return;
                    currentTasks = new List<Func<Task>>(_logQueue);
                    _logQueue.Clear();
                }

                foreach (var logAction in currentTasks)
                {
                    try
                    {
                        await logAction(); // Execute the actual file write
                    }
                    catch (Exception ex)
                    {
                        // Fallback for errors during individual log action
                        Console.WriteLine($"[FileLogger Internal Error] Failed to write log entry via queue: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback for errors during semaphore or queue manipulation
                Console.WriteLine($"[FileLogger Queue Error] Error processing log queue: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Actual async file writing logic, to be called by functions in the queue
        private async Task AppendLogToFileAsync(string level, string message)
        {
            string path = GetLogFilePathForDate(DateTime.Now);
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] - {message}{Environment.NewLine}";

            try
            {
                // File.AppendAllTextAsync is used here. For very high throughput scenarios,
                // a more complex system (e.g., dedicated writer thread, MemoryMappedFiles) might be needed.
                await File.AppendAllTextAsync(path, logEntry, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[FileLogger IO Error] Failed to write log entry to file '{path}': {ioEx.Message}. Entry: {logEntry.Trim()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileLogger Unexpected Error] Failed to write log entry to file '{path}': {ex.Message}. Entry: {logEntry.Trim()}");
            }
        }

        // New Enqueue method
        private void EnqueueLogAction(string level, string message)
        {
            lock (_queueLock)
            {
                _logQueue.Add(() => AppendLogToFileAsync(level, message));

                if (_loggingTask.IsCompleted)
                {
                    _loggingTask = ProcessLogQueueAsync();
                }
            }
            // CleanUpOldLogs() is removed from here. It's called in constructor and SetRetentionDays.
        }

        public void Log(string message)
        {
            EnqueueLogAction("資訊", message);
        }

        public void Log(string message, Exception ex)
        {
            EnqueueLogAction("錯誤", $"{message} - 例外: {ex}");
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                EnqueueLogAction("錯誤", $"{message} - 例外: {ex}");
            }
            else
            {
                EnqueueLogAction("錯誤", message);
            }
        }

        public void LogWarning(string message)
        {
            EnqueueLogAction("警告", message);
        }

        public void LogInformation(string message)
        {
            EnqueueLogAction("資訊", message);
        }

        public void LogDebug(string message)
        {
            EnqueueLogAction("偵錯", message);
        }
    }
}
