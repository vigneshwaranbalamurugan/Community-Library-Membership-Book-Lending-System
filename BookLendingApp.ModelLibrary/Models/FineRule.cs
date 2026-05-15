using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class FineRule
    {
        public Guid FineAmountId { get; set; }
        public FineType FineType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public FineCalculationType FineCalculationType { get; set; }
        public Decimal? Percentage { get; set; }
        public Decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}