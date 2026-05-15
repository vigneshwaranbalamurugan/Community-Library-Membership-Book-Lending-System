using System.ComponentModel.DataAnnotations;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class BorrowRecord
    {
        [Key]
        public Guid BorrowRecordId { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        [Required]
        public Guid BookCopyId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnDate { get; set; }

        [Range(0, int.MaxValue)]
        public int RenewalCount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int RenewalDays { get; set; } = 0; 

        [Range(1, int.MaxValue)]
        public BorrowStatus BorrowStatus { get; set; } = BorrowStatus.Active;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Range(typeof(decimal), "0", "100")]
        public decimal? DamagePercentage { get; set; }

        public Member? Member { get; set; }

        public BookCopy? BookCopy { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}