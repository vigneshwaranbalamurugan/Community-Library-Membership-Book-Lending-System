namespace BookLendingApp.ModelLibrary.Models
{
    public class Member
    {
        public Guid MemberId { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public Boolean IsActive { get; set; } = true;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        
        public Guid MembershipId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}