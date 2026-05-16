using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IMembershipService
    {
        MemberShip AddMembership(string name, int maxBooksAllowed, int maxBorrowDurationDays, bool isRenewalAllowed, int maxRenewalTimes, int maxRenewalDurationDays, decimal membershipFee);
        MemberShip UpdateMembership(Guid membershipId, string name, int maxBooksAllowed, int maxBorrowDurationDays, bool isRenewalAllowed, int maxRenewalTimes, int maxRenewalDurationDays, decimal membershipFee);
        void RemoveMembership(Guid membershipId);
        MemberShip GetMembershipById(Guid membershipId);
        List<MemberShip> GetAllMemberships();
        bool UpdateMembershipMaxBooks(Guid membershipId, int maxBooks);
        bool UpdateMembershipDuration(Guid membershipId, int durationInDays);
    }
}