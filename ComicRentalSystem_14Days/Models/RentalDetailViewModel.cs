using System;

namespace ComicRentalSystem_14Days.Models
{
    public class RentalDetailViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberPhoneNumber { get; set; } = string.Empty;

        public int ComicId { get; set; }
        public string ComicTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public DateTime? RentalDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnTime { get; set; }
    }
}
