using System.ComponentModel.DataAnnotations;

namespace BookLendingApp.ModelLibrary.Models
{
    public class Member
    {
        [Key]
        public Guid MemberId { get; set; }

        [Required]
        [StringLength(200)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public required string EmailId { get; set; }

        public Boolean IsActive { get; set; } = true;

        [Phone]
        [StringLength(20)]
        public string? MobileNumber { get; set; }

        [Required]
        [StringLength(255)]
        public required string Password { get; set; }
        

        [Required]
        public Guid MembershipId { get; set; }

        public MemberShip? Membership { get; set; }

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Member()
        {
            
        }

        public Member(string fullName, string emailId, string password, Guid membershipId, string? mobileNumber = null)
        {
            MemberId = Guid.NewGuid();
            FullName = fullName;
            EmailId = emailId;
            Password = password;
            MembershipId = Guid.Parse(membershipId.ToString());
            MobileNumber = mobileNumber;
        }

    }
}