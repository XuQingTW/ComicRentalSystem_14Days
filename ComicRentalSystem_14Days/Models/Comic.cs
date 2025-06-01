using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Models
{
    public class Comic : BaseEntity 
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Genre { get; set; }
        public bool IsRented { get; set; }
        public int RentedToMemberId { get; set; }
        public DateTime? RentalDate { get; set; } 
        public DateTime? ReturnDate { get; set; } 
        public DateTime? ActualReturnTime { get; set; }

        public Comic()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Genre = string.Empty;
            RentalDate = null;
            ReturnDate = null;
            ActualReturnTime = null;
        }

        public string ToCsvString()
        {
            var rentalDateString = RentalDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            var returnDateString = ReturnDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            var actualReturnTimeString = ActualReturnTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty;
            return $"{Id},\"{Title?.Replace("\"", "\"\"")}\",\"{Author?.Replace("\"", "\"\"")}\",\"{Isbn?.Replace("\"", "\"\"")}\",\"{Genre?.Replace("\"", "\"\"")}\",{IsRented},{RentedToMemberId},{rentalDateString},{returnDateString},{actualReturnTimeString}";
        }

        public static Comic FromCsvString(string csvLine)
        {
            List<string> values = ParseCsvLine(csvLine); 
            if (values.Count < 7) 
            {
                throw new FormatException("CSV 行未包含足夠的漫畫欄位值 (至少需要 7 個)。行: " + csvLine);
            }

            Comic comic = new Comic();
            try 
            {
                comic.Id = int.Parse(values[0]);
                comic.Title = values[1];
                comic.Author = values[2];
                comic.Isbn = values[3];
                comic.Genre = values[4];
                comic.IsRented = bool.Parse(values[5]);
                comic.RentedToMemberId = string.IsNullOrEmpty(values[6]) ? 0 : int.Parse(values[6]);

                if (values.Count > 7 && !string.IsNullOrEmpty(values[7]))
                {
                    if (DateTime.TryParse(values[7], out DateTime rentalDate))
                    {
                        comic.RentalDate = rentalDate;
                    }
                    else
                    {
                        comic.RentalDate = null;
                    }
                }
                else
                {
                    comic.RentalDate = null;
                }

                if (values.Count > 8 && !string.IsNullOrEmpty(values[8]))
                {
                    if (DateTime.TryParse(values[8], out DateTime returnDate))
                    {
                        comic.ReturnDate = returnDate;
                    }
                    else
                    {
                        comic.ReturnDate = null;
                    }
                }
                else
                {
                    comic.ReturnDate = null;
                }

                if (values.Count > 9 && !string.IsNullOrEmpty(values[9]))
                {
                    if (DateTime.TryParse(values[9], out DateTime actualReturnTime))
                    {
                        comic.ActualReturnTime = actualReturnTime;
                    }
                    else
                    {
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
                throw new Exception($"解析漫畫 CSV 時發生一般錯誤: {ex.Message}，行: {csvLine}", ex);
            }
            return comic;
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