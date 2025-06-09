using System;
using System.Collections.Generic;
// Potentially remove System.Linq, System.Text, System.Threading.Tasks if not used elsewhere

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
        // CSV related methods removed
    }
}