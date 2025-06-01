using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Models
{
    public class Member : BaseEntity 
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; } 
        public string Username { get; set; } 


        public Member()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Username = string.Empty;
        }
        public string ToCsvString()
        {
            return $"{Id},\"{Name?.Replace("\"", "\"\"")}\",\"{PhoneNumber?.Replace("\"", "\"\"")}\",\"{Username?.Replace("\"", "\"\"")}\"";
        }

        public static Member FromCsvString(string csvLine)
        {
            List<string> values = ParseCsvLine(csvLine); 
            if (values.Count < 3)
            {
                throw new FormatException("CSV 行未包含足夠的會員欄位值 (至少需要ID、姓名、電話號碼)。行: " + csvLine);
            }

            Member member = new Member();
            try 
            {
                member.Id = int.Parse(values[0]);
                member.Name = values[1];
                member.PhoneNumber = values[2];

                if (values.Count > 3)
                {
                    member.Username = values[3];
                }
                else
                {
                    member.Username = member.Name;
                }
            }
            catch (FormatException ex)
            {
                throw new FormatException($"解析會員 CSV 行時發生錯誤: '{csvLine}'。詳細資訊: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"解析會員 CSV 時發生一般錯誤: {ex.Message}，行: {csvLine}", ex);
            }
            return member;
        }

        internal static List<string> ParseCsvLine(string csvLine) 
        {
            List<string> fields = new List<string>();
            StringBuilder fieldBuilder = new StringBuilder();
            bool inQuotes = false;
            bool currentFieldWasQuoted = false; 
            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                        {
                            fieldBuilder.Append('"'); 
                            i++; 
                        }
                        else
                        {
                            inQuotes = false; 
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
                        if (fieldBuilder.Length == 0)
                        {
                            inQuotes = true;
                            currentFieldWasQuoted = true; 
                        }
                        else
                        {
                            fieldBuilder.Append(c);
                        }
                    }
                    else if (c == ',')
                    {
                        if (currentFieldWasQuoted)
                        {
                            fields.Add(fieldBuilder.ToString()); 
                        }
                        else
                        {
                            fields.Add(fieldBuilder.ToString().Trim());
                        }
                        fieldBuilder.Clear();
                        currentFieldWasQuoted = false;
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }
            if (currentFieldWasQuoted)
            {
                fields.Add(fieldBuilder.ToString()); 
            }
            else
            {
                fields.Add(fieldBuilder.ToString().Trim());
            }
            return fields;
        }
    }
}


