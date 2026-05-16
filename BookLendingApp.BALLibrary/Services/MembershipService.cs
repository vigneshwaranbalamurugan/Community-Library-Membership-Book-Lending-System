using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;

        public MembershipService(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public MemberShip AddMembership(string name, int maxBooksAllowed, int maxBorrowDurationDays, bool isRenewalAllowed, int maxRenewalTimes, int maxRenewalDurationDays, decimal membershipFee)
        {
            var membership = new MemberShip
            {
                Name = name,
                MaxBooksAllowed = maxBooksAllowed,
                MaxBorrowDurationDays = maxBorrowDurationDays,
                IsRenewalAllowed = isRenewalAllowed,
                MaxRenewalTimes = maxRenewalTimes,
                MaxRenewalDurationDays = maxRenewalDurationDays,
                MembershipFee = membershipFee
            };
            return _membershipRepository.Create(membership);
        }

        public MemberShip UpdateMembership(Guid membershipId, string name, int maxBooksAllowed, int maxBorrowDurationDays, bool isRenewalAllowed, int maxRenewalTimes, int maxRenewalDurationDays, decimal membershipFee)
        {
            var updated = new MemberShip
            {
                MemberShipId = membershipId,
                Name = name,
                MaxBooksAllowed = maxBooksAllowed,
                MaxBorrowDurationDays = maxBorrowDurationDays,
                IsRenewalAllowed = isRenewalAllowed,
                MaxRenewalTimes = maxRenewalTimes,
                MaxRenewalDurationDays = maxRenewalDurationDays,
                MembershipFee = membershipFee
            };
            return _membershipRepository.Update(membershipId, updated);
        }

        public void RemoveMembership(Guid membershipId) => _membershipRepository.Delete(membershipId);

        public MemberShip GetMembershipById(Guid membershipId)
        {
            return _membershipRepository.Get(membershipId) ?? throw new KeyNotFoundException($"Membership {membershipId} not found.");
        }

        public List<MemberShip> GetAllMemberships() => _membershipRepository.GetAll();

        public bool UpdateMembershipMaxBooks(Guid membershipId, int maxBooks) => _membershipRepository.UpdateMebershipMaxBooks(membershipId, maxBooks);

        public bool UpdateMembershipDuration(Guid membershipId, int durationInDays) => _membershipRepository.UpdateMembershipDuration(membershipId, durationInDays);
    }
}