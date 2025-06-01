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
        public string Username { get; set; } // 已新增 Username 屬性

        // 建構函式
        public Member()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Username = string.Empty; // 初始化 Username
        }

        // 將 Member 物件轉換為 CSV 格式的一行字串
        public string ToCsvString()
        {
            // 簡單起見，假設姓名和電話號碼不包含逗號或雙引號
            // 實際應用中，對包含特殊字元的欄位應使用雙引號包裹，並對欄位內的雙引號轉義
            // 已將 Username 新增至 CSV 輸出
            return $"{Id},\"{Name?.Replace("\"", "\"\"")}\",\"{PhoneNumber?.Replace("\"", "\"\"")}\",\"{Username?.Replace("\"", "\"\"")}\"";
        }

        // 從 CSV 格式的一行字串解析回 Member 物件
        public static Member FromCsvString(string csvLine)
        {
            List<string> values = ParseCsvLine(csvLine); // 使用新的解析器
            // 最少 3 個欄位 (Id、Name、PhoneNumber)，Username 為可選，以實現向後相容性
            if (values.Count < 3)
            {
                throw new FormatException("CSV 行未包含足夠的會員欄位值 (至少需要ID、姓名、電話號碼)。行: " + csvLine);
            }

            Member member = new Member();
            try // 技術點 #5: 例外處理
            {
                member.Id = int.Parse(values[0]);
                member.Name = values[1];
                member.PhoneNumber = values[2];

                // 處理 Username (新欄位) - 向後相容性
                if (values.Count > 3)
                {
                    member.Username = values[3];
                }
                else
                {
                    // 舊版 CSV 項目的備用方案：使用 Name 作為 Username
                    // 這與先前 Name 隱含作為使用者名稱連結的行為保持一致
                    member.Username = member.Name;
                }
            }
            catch (FormatException ex)
            {
                throw new FormatException($"解析會員 CSV 行時發生錯誤: '{csvLine}'。詳細資訊: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // 最好擲回特定的例外狀況或適當地記錄錯誤。
                throw new Exception($"解析會員 CSV 時發生一般錯誤: {ex.Message}，行: {csvLine}", ex);
            }
            return member;
        }

        // 解析 CSV 行的輔助方法，可處理引號欄位和逸出引號。
        // 此方法與 Comic.cs 中的方法相同。
        // 如果更多模型需要此功能，請考慮重構為共用公用程式類別。
        internal static List<string> ParseCsvLine(string csvLine) // 為方便測試，已從 private 改為 internal
        {
            List<string> fields = new List<string>();
            StringBuilder fieldBuilder = new StringBuilder();
            bool inQuotes = false;
            bool currentFieldWasQuoted = false; // 已新增旗標
            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                        {
                            fieldBuilder.Append('"'); // 已轉義的引號
                            i++; // 跳過下一個引號
                        }
                        else
                        {
                            inQuotes = false; // 引號欄位結束
                        }
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        // 標準 CSV 預期引號位於欄位的開頭。
                        // 如果 fieldBuilder 有內容，表示引號不在開頭。
                        // 這可能是未加引號欄位中的引號，或是格式錯誤。
                        // 對於此解析器，我們將欄位開頭 (逗號或行首之後) 以外的引號
                        // 在非 `inQuotes` 模式下視為文字字元。
                        if (fieldBuilder.Length == 0) // 引號位於新欄位的開頭
                        {
                            inQuotes = true;
                            currentFieldWasQuoted = true; // 將欄位標記為已加引號
                        }
                        else
                        {
                            // 引號不在欄位值的開頭，因此它是文字引號。
                            fieldBuilder.Append(c);
                        }
                    }
                    else if (c == ',')
                    {
                        if (currentFieldWasQuoted)
                        {
                            fields.Add(fieldBuilder.ToString()); // 若有引號則照原樣新增
                        }
                        else
                        {
                            fields.Add(fieldBuilder.ToString().Trim()); // 若無引號則修剪
                        }
                        fieldBuilder.Clear();
                        currentFieldWasQuoted = false; // 為下一個欄位重設
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }
            // 新增最後一個欄位
            if (currentFieldWasQuoted)
            {
                fields.Add(fieldBuilder.ToString()); // 若有引號則照原樣新增
            }
            else
            {
                fields.Add(fieldBuilder.ToString().Trim()); // 若無引號則修剪
            }
            return fields;
        }
    }
}


