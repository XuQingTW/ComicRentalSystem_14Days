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
        public DateTime? RentalDate { get; set; } // 已新增
        public DateTime? ReturnDate { get; set; } // 已新增
        public DateTime? ActualReturnTime { get; set; } // 已新增

        public Comic()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Genre = string.Empty;
            RentalDate = null; // 初始化
            ReturnDate = null; // 初始化
            ActualReturnTime = null; // 初始化
        }

        // 將 Comic 物件轉換為 CSV 格式的一行字串
        // 技術點 #1: 字串
        public string ToCsvString()
        {
            var rentalDateString = RentalDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            var returnDateString = ReturnDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            var actualReturnTimeString = ActualReturnTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            return $"{Id},\"{Title?.Replace("\"", "\"\"")}\",\"{Author?.Replace("\"", "\"\"")}\",\"{Isbn?.Replace("\"", "\"\"")}\",\"{Genre?.Replace("\"", "\"\"")}\",{IsRented},{RentedToMemberId},{rentalDateString},{returnDateString},{actualReturnTimeString}";
        }

        // 從 CSV 格式的一行字串解析回 Comic 物件
        // 技術點 #1: 字串與陣列
        public static Comic FromCsvString(string csvLine)
        {
            List<string> values = ParseCsvLine(csvLine); // 使用新的解析器
            // Expect at least 7 fields (original) or up to 10 fields (with new dates)
            if (values.Count < 7) // 允許 7 到 10 個欄位。舊資料可能有 7 個，較新的資料最多 10 個。
            {
                throw new FormatException("CSV 行未包含足夠的漫畫欄位值 (至少需要 7 個)。行: " + csvLine);
            }

            Comic comic = new Comic();
            try // 技術點 #5: 例外處理
            {
                comic.Id = int.Parse(values[0]);
                comic.Title = values[1];
                comic.Author = values[2];
                comic.Isbn = values[3];
                comic.Genre = values[4];
                comic.IsRented = bool.Parse(values[5]);
                comic.RentedToMemberId = string.IsNullOrEmpty(values[6]) ? 0 : int.Parse(values[6]);

                // 處理可選的 RentalDate (欄位 8，索引 7)
                if (values.Count > 7 && !string.IsNullOrEmpty(values[7]))
                {
                    if (DateTime.TryParse(values[7], out DateTime rentalDate))
                    {
                        comic.RentalDate = rentalDate;
                    }
                    else
                    {
                        // 可選擇性地記錄或處理 RentalDate 的解析錯誤
                        comic.RentalDate = null;
                    }
                }
                else
                {
                    comic.RentalDate = null;
                }

                // 處理可選的 ReturnDate (欄位 9，索引 8)
                if (values.Count > 8 && !string.IsNullOrEmpty(values[8]))
                {
                    if (DateTime.TryParse(values[8], out DateTime returnDate))
                    {
                        comic.ReturnDate = returnDate;
                    }
                    else
                    {
                        // 可選擇性地記錄或處理 ReturnDate 的解析錯誤
                        comic.ReturnDate = null;
                    }
                }
                else
                {
                    comic.ReturnDate = null;
                }

                // 處理可選的 ActualReturnTime (欄位 10，索引 9)
                if (values.Count > 9 && !string.IsNullOrEmpty(values[9]))
                {
                    if (DateTime.TryParse(values[9], out DateTime actualReturnTime))
                    {
                        comic.ActualReturnTime = actualReturnTime;
                    }
                    else
                    {
                        // 可選擇性地記錄或處理 ActualReturnTime 的解析錯誤
                        comic.ActualReturnTime = null;
                    }
                }
                else
                {
                    comic.ActualReturnTime = null;
                }
            }
            catch (FormatException ex)
            {
                throw new FormatException($"解析漫畫 CSV 行時發生錯誤: '{csvLine}'。詳細資訊: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // 最好擲回特定的例外狀況或適當地記錄錯誤。
                // 目前，重新擲回原始例外狀況以保持行為，同時指出攔截位置。
                throw new Exception($"解析漫畫 CSV 時發生一般錯誤: {ex.Message}，行: {csvLine}", ex);
            }
            return comic;
        }

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
                        // 引號欄位的開始。如果 fieldBuilder 不是空的，
                        // 這表示引號不在欄位的開頭，
                        // 這對於標準 CSV 來說不尋常。然而，我們將其視為欄位的一部分。
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
                            // 如果 CSV 未嚴格格式化引號欄位 (例如 field"with"quote)，則可能發生這種情況
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
            // 新增最後一個欄位，確保修剪空白。
            // 如果最後一個欄位已加引號，則定義其的引號由 inQuotes 邏輯處理。
            // 任何內部引號 (已轉義) 都會正確保留。
            // 引號欄位周圍的外部空白 (例如 " value " ,) 在新增前也應修剪。
            // 此處的 .Trim() 會處理最後一個欄位的空白，無論其是否已加引號。
            if (currentFieldWasQuoted)
            {
                fields.Add(fieldBuilder.ToString()); // 若有引號則照原樣新增
            }
            else
            {
                fields.Add(fieldBuilder.ToString().Trim()); // 若無引號則修剪
            }

            // 後期處理：目前的解析器旨在移除結構性引號。
            // 如果邏輯正確，則無需明確的 Trim('"')。
            // 欄位應包含其實際值。
            return fields;
        }
    }
}