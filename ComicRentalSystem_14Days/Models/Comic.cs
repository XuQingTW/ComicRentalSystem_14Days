using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Models
{
    public class Comic : BaseEntity // 繼承自 BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Genre { get; set; }
        public bool IsRented { get; set; }
        public int RentedToMemberId { get; set; } // 0 or specific MemberId if rented

        // 建構函式
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
            // 為避免CSV中的逗號問題，可以考慮將包含逗號的欄位用引號包起來
            // 但為了簡單起見，這裡假設欄位內容本身不含逗號或換行符
            // 實際應用中可能需要更 robust 的 CSV 處理庫或手動處理引號
            return $"{Id},\"{Title?.Replace("\"", "\"\"")}\",\"{Author?.Replace("\"", "\"\"")}\",\"{Isbn?.Replace("\"", "\"\"")}\",\"{Genre?.Replace("\"", "\"\"")}\",{IsRented},{RentedToMemberId}";
        }

        // 從 CSV 格式的一行字串解析回 Comic 物件
        // 技術點 #1: 字串與陣列
        // 這個靜態方法將被 FileHelper 使用
        public static Comic FromCsvString(string csvLine)
        {
            // 這裡需要一個更可靠的CSV解析方式，特別是如果欄位值可能包含逗號或引號
            // 簡單的 Split(',') 在這種情況下會出錯。
            // 為了教學目的和時間限制，我們先用一個簡化的 Split，但請注意其限制。
            // 更好的方法是使用正規表示式或一個簡單的狀態機來解析CSV。

            // 簡易CSV解析 (僅適用於不包含引號內逗號的簡單情況)
            // 若要處理 "field1","field,with,comma","field3" 這種情況，需要更複雜的解析
            string[] values = csvLine.Split(',');
            if (values.Length < 7)
            {
                // 嘗試另一種解析，考慮到 Title 等欄位可能用引號包起來且內部有逗號 (但我們 ToCsvString 沒這麼做)
                // 為了簡單，這裡直接拋錯
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