namespace BookLendingApp.ModelLibrary.Models
{
    public class MemberBorrowingSummary
    {
        public long ActiveBorrows { get; set; }
        public long ReturnedBorrows { get; set; }
        public decimal TotalUnpaidFine { get; set; }
    }
}