using System.ComponentModel.DataAnnotations;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class BookCopy
    {
        [Key]
        public Guid BookCopyId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Barcode { get; set; }

        [Required]
        [StringLength(100)]
        public required string ShelfLocation { get; set; }

        [Range(1, int.MaxValue)]
        public BookStatus Status { get; set; } = BookStatus.Available;

        [Range(typeof(decimal), "0", "100")]
        public Decimal DamagePercentage { get; set; } = 0;

        [StringLength(500)]
        public string? Condition { get; set; }

        public Book? Book { get; set; }

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public BookCopy()
        {
            
        }

        public BookCopy(Guid bookId, string barcode, string shelfLocation, BookStatus status = BookStatus.Available, decimal damagePercentage = 0, string? condition = null)
        {
            BookCopyId = Guid.NewGuid();
            BookId = bookId;
            Barcode = barcode;
            ShelfLocation = shelfLocation;
            Status = status;
            DamagePercentage = damagePercentage;
            Condition = condition;
        }
        
    }
}