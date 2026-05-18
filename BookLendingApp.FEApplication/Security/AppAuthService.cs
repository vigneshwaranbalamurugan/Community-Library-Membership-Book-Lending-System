using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.FEApplication.Security
{
    public sealed class AppAuthService
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "Admin@123";

        private readonly IMemberService _memberService;

        public AppAuthService(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public bool ValidateAdmin(string username, string password)
        {
            return string.Equals(username.Trim(), AdminUsername, StringComparison.OrdinalIgnoreCase)
                && string.Equals(password, AdminPassword, StringComparison.Ordinal);
        }

        public Member AuthenticateMember(string email, string password)
        {
            var member = _memberService.GetMemberByEmail(email.Trim());

            if (!member.IsActive)
            {
                throw new UnauthorizedAccessException("Your account is inactive.");
            }

            if (!string.Equals(member.Password, password, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return member;
        }
    }
}