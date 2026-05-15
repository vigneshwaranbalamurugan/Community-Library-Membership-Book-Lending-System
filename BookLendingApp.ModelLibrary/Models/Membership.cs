namespace BookLendingApp.ModelLibrary.Models
{
    public class MemberShip
    {
        public Guid MemberShipId{get;set;}
        public string Name { get; set; }
        public int MaxBooksAllowed { get; set; }
        public int MaxBorrowDurationDays { get; set; }
        public bool IsRenewalAllowed { get; set; }
        public int MaxRenewalTimes { get; set; }
        public int MaxRenewalDurationDays { get; set; }
        public Decimal MembershipFee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    
}