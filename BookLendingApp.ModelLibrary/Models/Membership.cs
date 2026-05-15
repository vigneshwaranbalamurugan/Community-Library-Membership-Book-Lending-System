using System.ComponentModel.DataAnnotations;

namespace BookLendingApp.ModelLibrary.Models
{
    public class MemberShip
    {
        [Key]
        public Guid MemberShipId{get;set;}

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxBooksAllowed { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxBorrowDurationDays { get; set; }

        public bool IsRenewalAllowed { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxRenewalTimes { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxRenewalDurationDays { get; set; }

        [Range(typeof(decimal), "0", "999999999999.99")]
        public Decimal MembershipFee { get; set; }

        public ICollection<Member> Members { get; set; } = new List<Member>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
}