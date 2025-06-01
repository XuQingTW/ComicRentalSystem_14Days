using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Text;

namespace ComicRentalSystem_14Days.Logging 
{
    // 技術點3: 介面
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object(); 

        public FileLogger(string logFileName = "application_log.txt")
        {
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex) // 技術點5: 例外處理
            {
                Console.WriteLine($"[嚴重] 無法建立日誌目錄 '{logDirectory}': {ex.Message}");
                logDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            _logFilePath = Path.Combine(logDirectory, logFileName);
        }

        // 技術點3: 介面 (實作 ILogger 的 Log 方法)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void Log(string message)
        {
            WriteLogEntry("資訊", message);
        }

        // 技術點3: 介面 (實作 ILogger 的 Log 過載方法)
        // 技術點4: 過載 (FileLogger 中的 Log 方法過載)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void Log(string message, Exception ex)
        {
            WriteLogEntry("錯誤", $"{message} - 例外: {ex.ToString()}");
        }

        // 技術點3: 介面 (實作 ILogger 的 LogError 過載方法)
        // 技術點4: 過載 (FileLogger 中的 LogError 方法)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                WriteLogEntry("錯誤", $"{message} - 例外: {ex.ToString()}");
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
            // 技術點8: 檔案與資料夾處理
            // 技術點5: 例外處理 (try-catch)
            try
            {
                lock (_lock)
                {
                    // 使用 UTF-8 編碼
                    File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] - {message}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[檔案記錄器 IO 錯誤] 無法寫入日誌檔案 '{_logFilePath}': {ioEx.Message}。原始訊息: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[檔案記錄器 未預期錯誤] 無法寫入日誌檔案 '{_logFilePath}': {ex.Message}。原始訊息: {message}");
            }
        }
    }
}