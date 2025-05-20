using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Text;

namespace ComicRentalSystem_14Days.Logging // 或者 ComicRentalSystem_14Days.Helpers
{
    // 技術點3: 介面 (實作 ILogger)
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object(); // 用於執行緒安全

        public FileLogger(string logFileName = "application_log.txt")
        {
            // 將日誌檔案存放在 "我的文件" 資料夾下的應用程式特定資料夾中
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
                Console.WriteLine($"[CRITICAL] Failed to create log directory '{logDirectory}': {ex.Message}");
                // 在無法建立日誌目錄的嚴重情況下，可以考慮退回到應用程式根目錄或其他備案
                logDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            _logFilePath = Path.Combine(logDirectory, logFileName);
        }

        // 技術點3: 介面 (實作 ILogger 的 Log 方法)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void Log(string message)
        {
            WriteLogEntry("INFO", message);
        }

        // 技術點3: 介面 (實作 ILogger 的 Log 過載方法)
        // 技術點4: 過載 (FileLogger 中的 Log 方法過載)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void Log(string message, Exception ex)
        {
            WriteLogEntry("ERROR", $"{message} - Exception: {ex.ToString()}");
        }

        // 技術點3: 介面 (實作 ILogger 的 LogError 過載方法)
        // 技術點4: 過載 (FileLogger 中的 LogError 方法)
        // 技術點4: 多型 (當透過 ILogger 引用呼叫此方法時)
        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                WriteLogEntry("ERROR", $"{message} - Exception: {ex.ToString()}");
            }
            else
            {
                WriteLogEntry("ERROR", message);
            }
        }

        private void WriteLogEntry(string level, string message)
        {
            // 技術點8: 檔案與資料夾處理
            // 技術點5: 例外處理 (try-catch)
            try
            {
                // lock 確保多執行緒環境下檔案寫入的原子性
                lock (_lock)
                {
                    // File.AppendAllText 會自動處理檔案的建立 (如果不存在)
                    // 使用 UTF-8 編碼
                    File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] - {message}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch (IOException ioEx)
            {
                // 檔案寫入錯誤，可以嘗試輸出到主控台作為備案
                Console.WriteLine($"[FileLogger IO Error] Failed to write to log file '{_logFilePath}': {ioEx.Message}. Original message: {message}");
            }
            catch (Exception ex)
            {
                // 其他未預期的錯誤
                Console.WriteLine($"[FileLogger Unexpected Error] Failed to write to log file '{_logFilePath}': {ex.Message}. Original message: {message}");
            }
        }
    }
}