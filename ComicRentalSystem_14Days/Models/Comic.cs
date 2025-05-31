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
            List<string> values = ParseCsvLine(csvLine); // Use the new parser
            if (values.Count < 7)
            {
                throw new FormatException("CSV line does not contain enough values for Comic. Line: " + csvLine);
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
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Error parsing CSV line for Comic: '{csvLine}'. Details: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // It's better to throw a specific exception or log the error appropriately.
                // For now, rethrow the original exception to maintain behavior while indicating where it was caught.
                throw new Exception($"Generic error parsing comic CSV: {ex.Message} for line: {csvLine}", ex);
            }
            return comic;
        }

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
                        // Start of a quoted field. If fieldBuilder is not empty,
                        // it means the quote is not at the start of the field,
                        // which is unusual for standard CSV. However, we'll treat it as part of the field.
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
                            // This can happen if the CSV is not strictly formatting quoted fields, e.g. field"with"quote
                            fieldBuilder.Append(c);
                        }
                    }
                    else if (c == ',')
                    {
                        fields.Add(fieldBuilder.ToString().Trim()); // Trim whitespace for unquoted fields or already processed quoted fields
                        fieldBuilder.Clear();
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }
            // Add the last field, ensuring whitespace is trimmed.
            // If the last field was quoted, the quotes defining it as such are handled by inQuotes logic.
            // Any internal quotes (escaped) are preserved correctly.
            // External whitespace around a quoted field (e.g., " value " ,) should also be trimmed before adding.
            // The .Trim() here handles whitespace for the last field, whether it was quoted or not.
            fields.Add(fieldBuilder.ToString().Trim());

            // Post-processing: The current parser aims to remove structural quotes.
            // No explicit Trim('"') should be necessary if the logic is correct.
            // The fields should contain their actual values.
            return fields;
        }
    }
}