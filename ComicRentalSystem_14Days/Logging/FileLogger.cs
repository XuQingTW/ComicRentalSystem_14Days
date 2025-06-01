using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Text;

namespace ComicRentalSystem_14Days.Logging 
{
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
            catch (Exception ex) 
            {
                Console.WriteLine($"[嚴重] 無法建立日誌目錄 '{logDirectory}': {ex.Message}");
                logDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            _logFilePath = Path.Combine(logDirectory, logFileName);
        }

        public void Log(string message)
        {
            WriteLogEntry("資訊", message);
        }

        public void Log(string message, Exception ex)
        {
            WriteLogEntry("錯誤", $"{message} - 例外: {ex.ToString()}");
        }

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
            try
            {
                lock (_lock)
                {
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