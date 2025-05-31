using System;

namespace ComicRentalSystem_14Days.Models
{
    public class RentalDetailViewModel
    {
        // Member Details
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberPhoneNumber { get; set; } = string.Empty;

        // Comic Details
        public int ComicId { get; set; }
        public string ComicTitle { get; set; } = string.Empty;

        // Rental Details
        public DateTime? RentalDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
    }
}
