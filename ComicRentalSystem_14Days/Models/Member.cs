using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Models
{
    public class Member : BaseEntity // 繼承自 BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; } // 技術點 #1: 字串

        // 建構函式
        public Member()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
        }

        // 將 Member 物件轉換為 CSV 格式的一行字串
        public string ToCsvString()
        {
            // 簡單起見，假設姓名和電話號碼不包含逗號或雙引號
            // 實際應用中，對包含特殊字元的欄位應使用雙引號包裹，並對欄位內的雙引號轉義
            return $"{Id},\"{Name?.Replace("\"", "\"\"")}\",\"{PhoneNumber?.Replace("\"", "\"\"")}\"";
        }

        // 從 CSV 格式的一行字串解析回 Member 物件
        public static Member FromCsvString(string csvLine)
        {
            // 簡易CSV解析 (僅適用於不包含引號內逗號的簡單情況)
            string[] values = csvLine.Split(',');
            if (values.Length < 3)
            {
                throw new FormatException("CSV line does not contain enough values for Member. Line: " + csvLine);
            }

            Member member = new Member();
            try // 技術點 #5: 例外處理
            {
                member.Id = int.Parse(values[0].Trim());
                member.Name = values[1].Trim().Trim('"'); // 移除前後可能存在的引號
                member.PhoneNumber = values[2].Trim().Trim('"');
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Error parsing CSV line for Member: '{csvLine}'. Details: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic error parsing member CSV: {ex.Message} for line: {csvLine}");
                throw;
            }
            return member;
        }
    }
}


