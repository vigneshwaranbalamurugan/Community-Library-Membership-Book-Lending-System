using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.FEApplication.Security
{
    public sealed class AppSession
    {
        public UserRole CurrentRole { get; private set; } = UserRole.None;

        public Member? CurrentMember { get; private set; }

        public bool IsAuthenticated => CurrentRole != UserRole.None;

        public bool IsAdmin => CurrentRole == UserRole.Admin;

        public bool IsMember => CurrentRole == UserRole.Member && CurrentMember != null;

        public void LoginAsAdmin()
        {
            CurrentRole = UserRole.Admin;
            CurrentMember = null;
        }

        public void LoginAsMember(Member member)
        {
            CurrentRole = UserRole.Member;
            CurrentMember = member;
        }

        public void Logout()
        {
            CurrentRole = UserRole.None;
            CurrentMember = null;
        }
    }
}