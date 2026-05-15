using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IMembershipRepository : IRepository<Guid, MemberShip>
    {
        bool UpdateMebershipMaxBooks(Guid membershipId, int maxBooks);
        bool UpdateMembershipDuration(Guid membershipId, int durationInMonths);
    }
}