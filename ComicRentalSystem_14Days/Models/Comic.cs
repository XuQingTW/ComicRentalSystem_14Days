using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Models
{
    public class Comic : BaseEntity // 繼承 BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Genre { get; set; }
        public bool IsRented { get; set; }
        public int RentedToMemberId { get; set; }

        public Comic()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Genre = string.Empty;
        }

        // 將 Comic 物件轉換為 CSV 格式的一行字串
        // 技術點 #1: 字串
        public string ToCsvString()
        {
            return $"{Id},\"{Title?.Replace("\"", "\"\"")}\",\"{Author?.Replace("\"", "\"\"")}\",\"{Isbn?.Replace("\"", "\"\"")}\",\"{Genre?.Replace("\"", "\"\"")}\",{IsRented},{RentedToMemberId}";
        }

        // 從 CSV 格式的一行字串解析回 Comic 物件
        // 技術點 #1: 字串與陣列
        public static Comic FromCsvString(string csvLine)
        {
            string[] values = csvLine.Split(',');
            if (values.Length < 7)
            {
                throw new FormatException("CSV line does not contain enough values for Comic. Line: " + csvLine);
            }

            Comic comic = new Comic();
            try // 技術點 #5: 例外處理
            {
                comic.Id = int.Parse(values[0].Trim());
                // 移除前後可能存在的引號
                comic.Title = values[1].Trim().Trim('"');
                comic.Author = values[2].Trim().Trim('"');
                comic.Isbn = values[3].Trim().Trim('"');
                comic.Genre = values[4].Trim().Trim('"');
                comic.IsRented = bool.Parse(values[5].Trim());
                comic.RentedToMemberId = int.Parse(values[6].Trim());
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Error parsing CSV line for Comic: '{csvLine}'. Details: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic error parsing comic CSV: {ex.Message} for line: {csvLine}");
                throw;
            }
            return comic;
        }
    }
}