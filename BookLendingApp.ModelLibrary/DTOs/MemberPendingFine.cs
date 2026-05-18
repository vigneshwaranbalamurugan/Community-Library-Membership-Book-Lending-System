using System;

namespace BookLendingApp.ModelLibrary.Models
{
    public class MemberPendingFine
    {
        public Guid MemberId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public decimal UnpaidAmount { get; set; }
    }
}
