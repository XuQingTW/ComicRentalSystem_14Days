using System;

namespace ComicRentalSystem_14Days.Models
{
    public class RentalDetailViewModel
    {
        // 會員詳細資料
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberPhoneNumber { get; set; } = string.Empty;

        // 漫畫詳細資料
        public int ComicId { get; set; }
        public string ComicTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        // 租借詳細資料
        public DateTime? RentalDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnTime { get; set; }
    }
}
