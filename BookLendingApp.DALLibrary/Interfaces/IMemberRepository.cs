using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IMemberRepository : IRepository<Guid, Member>
    {
        Member GetMemberByEmail(string email);
        Member GetMemberByPhone(string phone);

        bool UpdateMembership(Guid memberId, string newMembershipId);

        bool UpdateStatus(Guid memberId, bool isActive);
        List<Member> GetActiveMembers();
    }
}