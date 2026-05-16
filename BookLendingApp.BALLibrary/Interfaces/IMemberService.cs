using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IMemberService
    {
        Member AddMember(string fullName, string emailId, string password, Guid membershipId, string? mobileNumber = null);
        Member UpdateMember(Guid memberId, string fullName, string emailId, string password, Guid membershipId, bool isActive, string? mobileNumber = null);
        void RemoveMember(Guid memberId);
        Member GetMemberById(Guid memberId);
        List<Member> GetAllMembers();
        Member GetMemberByEmail(string email);
        Member GetMemberByPhone(string phone);
        bool UpdateMembership(Guid memberId, string newMembershipId);
        bool UpdateStatus(Guid memberId, bool isActive);
        List<Member> GetActiveMembers();
    }
}