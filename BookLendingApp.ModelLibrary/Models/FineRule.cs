using System.ComponentModel.DataAnnotations;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class FineRule
    {
        [Key]
        public Guid FineAmountId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public FineType FineType { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public FineCalculationType FineCalculationType { get; set; }

        [Range(typeof(decimal), "0", "100")]
        public Decimal? Percentage { get; set; }

        [Range(typeof(decimal), "0", "999999999999.99")]
        public Decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public FineRule()
        {
            
        }

        public FineRule(FineType fineType, string name, FineCalculationType fineCalculationType, decimal amount, string? description = null, decimal? percentage = null)
        {
            FineAmountId = Guid.NewGuid();
            FineType = fineType;
            Name = name;
            Description = description;
            FineCalculationType = fineCalculationType;
            Amount = amount;
            Percentage = percentage;
        }
    }
    
}