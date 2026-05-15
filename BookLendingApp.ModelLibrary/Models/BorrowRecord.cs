using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class BorrowRecord
    {
        public Guid BorrowRecordId { get; set; }
        public Guid MemberId { get; set; }
        public Guid BookCopyId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public int RenewalCount { get; set; } = 0;
        public int RenewalDays { get; set; } = 0; 
        public BorrowStatus BorrowStatus { get; set; } = BorrowStatus.Active;
        public string Notes{get;set;}
        public decimal? DamagePercentage{get;set;}
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}