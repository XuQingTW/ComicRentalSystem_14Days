using System;

namespace ComicRentalSystem_14Days.Models
{
    public class AdminComicStatusViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public string BorrowerPhoneNumber { get; set; } = string.Empty;
        public DateTime? RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ActualReturnTime { get; set; }
    }
}
