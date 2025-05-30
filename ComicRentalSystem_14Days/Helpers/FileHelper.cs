using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ComicRentalSystem_14Days.Helpers
{
    public class FileHelper
    {
        private readonly string _baseDataPath; // 資料檔案存放的基礎路徑

        public FileHelper(string baseDataFolderName = "AppData")
        {
            // 將資料檔案存放在 "我的文件" 資料夾下的指定子資料夾中
            // Environment.SpecialFolder.MyDocuments 取得 "我的文件" 路徑
            _baseDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", baseDataFolderName);

            // 確保資料夾存在
            try
            {
                if (!Directory.Exists(_baseDataPath))
                {
                    Directory.CreateDirectory(_baseDataPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating data directory '{_baseDataPath}': {ex.Message}");
                throw new IOException($"Failed to create or access data directory: {_baseDataPath}", ex);
            }
        }

        public string GetFullFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        public List<T> ReadCsvFile<T>(string fileName, Func<string, T> parseFunc)
        {
            string filePath = GetFullFilePath(fileName);
            List<T> records = new List<T>();

            if (!File.Exists(filePath))
            {
                return records;
            }

            try
            {
                // 技術點 #1: 字串與陣列 (雖然ReadAllLines已處理，但parseFunc中會用)
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                // 跳過標頭行 (如果有的話，這裡假設第一行是標頭)
                // records = lines.Skip(1).Select(line => parseFunc(line)).ToList();
                // 假設沒有標頭行
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue; // 跳過空行
                    records.Add(parseFunc(line));
                }
            }
            catch (IOException ioEx) // 技術點 #5: 例外處理
            {
                Console.WriteLine($"Error reading file '{filePath}': {ioEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading '{filePath}': {ex.Message}");
                throw;
            }
            return records;
        }

        // T 是要儲存的物件類型
        // toCsvFunc 是將 T 物件轉換為 CSV 字串行的函式
        public void WriteCsvFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvFunc)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                // 技術點 #1: 字串與陣列 (雖然WriteLine已處理，但toCsvFunc會產生字串)
                List<string> lines = new List<string>();
                foreach (var record in records)
                {
                    lines.Add(toCsvFunc(record));
                }
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException ioEx) // 技術點 #5: 例外處理
            {
                Console.WriteLine($"Error writing to file '{filePath}': {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while writing to '{filePath}': {ex.Message}");
                throw;
            }
        }
    }
}
