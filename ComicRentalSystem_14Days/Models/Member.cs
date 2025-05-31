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
            List<string> values = ParseCsvLine(csvLine); // Use the new parser
            if (values.Count < 3)
            {
                throw new FormatException("CSV line does not contain enough values for Member. Line: " + csvLine);
            }

            Member member = new Member();
            try // 技術點 #5: 例外處理
            {
                member.Id = int.Parse(values[0]);
                member.Name = values[1];
                member.PhoneNumber = values[2];
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Error parsing CSV line for Member: '{csvLine}'. Details: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // It's better to throw a specific exception or log the error appropriately.
                throw new Exception($"Generic error parsing member CSV: {ex.Message} for line: {csvLine}", ex);
            }
            return member;
        }

        // Helper method to parse a CSV line, handling quoted fields and escaped quotes.
        // This method is identical to the one in Comic.cs.
        // Consider refactoring to a shared utility class if more models need this.
        private static List<string> ParseCsvLine(string csvLine)
        {
            List<string> fields = new List<string>();
            StringBuilder fieldBuilder = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                        {
                            fieldBuilder.Append('"'); // Escaped quote
                            i++; // Skip next quote
                        }
                        else
                        {
                            inQuotes = false; // End of quoted field
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
                        // Standard CSV expects quotes at the beginning of a field.
                        // If fieldBuilder has content, it means the quote is not at the start.
                        // This could be a quote within an unquoted field or malformed.
                        // For this parser, we'll treat a quote not at the start of a field (after a comma or line start)
                        // as a literal character if not in `inQuotes` mode.
                        if (fieldBuilder.Length == 0)
                        {
                            inQuotes = true;
                        }
                        else
                        {
                            // Quote is not at the start of the field value, so it's a literal quote.
                            fieldBuilder.Append(c);
                        }
                    }
                    else if (c == ',')
                    {
                        fields.Add(fieldBuilder.ToString().Trim());
                        fieldBuilder.Clear();
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }
            fields.Add(fieldBuilder.ToString().Trim()); // Add the last field
            return fields;
        }
    }
}


