using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class BookCopy
    {
        public Guid BookCopyId { get; set; }
        public Guid BookId { get; set; }
        public string Barcode { get; set; }        
        public string ShelfLocation { get; set; }
        public BookStatus Status { get; set; } = BookStatus.Available;
        public Decimal DamagePercentage { get; set; } = 0;
        public string Condition { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}