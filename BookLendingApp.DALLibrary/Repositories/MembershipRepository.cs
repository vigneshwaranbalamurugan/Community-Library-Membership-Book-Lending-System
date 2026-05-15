using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class MembershipRepository: AbstractRepository<Guid, MemberShip>,IMembershipRepository
    {
        public MembershipRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override MemberShip Create(MemberShip item)
        {
            _context.MemberShips.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override MemberShip Update(Guid key, MemberShip item)
        {
            var existingMembership = _context.MemberShips.FirstOrDefault(m => m.MemberShipId == key);
            if (existingMembership == null)
                throw new KeyNotFoundException($"Membership with ID {key} not found.");
            
            existingMembership.Name = item.Name;
            existingMembership.MaxBooksAllowed = item.MaxBooksAllowed;
            existingMembership.MaxBorrowDurationDays = item.MaxBorrowDurationDays;
            existingMembership.IsRenewalAllowed = item.IsRenewalAllowed;
            existingMembership.MaxRenewalTimes = item.MaxRenewalTimes;
            existingMembership.MaxRenewalDurationDays = item.MaxRenewalDurationDays;
            existingMembership.MembershipFee = item.MembershipFee;
            existingMembership.UpdatedAt = DateTime.UtcNow;
            
            _context.SaveChanges();
            return existingMembership;
        }
        public override MemberShip? Get(Guid key)
        {
            return _context.MemberShips.FirstOrDefault(m => m.MemberShipId == key);
        }
        public override List<MemberShip> GetAll()
        {
            return _context.MemberShips.ToList();
        }
        public override MemberShip Delete(Guid key)
        {
            var membership = _context.MemberShips.FirstOrDefault(m => m.MemberShipId == key);
            if (membership == null)
                throw new KeyNotFoundException($"Membership with ID {key} not found.");
            
            _context.MemberShips.Remove(membership);
            _context.SaveChanges();
            return membership;
        }

        public bool UpdateMebershipMaxBooks(Guid membershipId, int maxBooks)
        {
            var membership = _context.MemberShips.FirstOrDefault(m => m.MemberShipId == membershipId);
            if (membership == null) return false;
            membership.MaxBooksAllowed = maxBooks;
            membership.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }

        public bool UpdateMembershipDuration(Guid membershipId, int durationInDays)
        {
            var membership = _context.MemberShips.FirstOrDefault(m => m.MemberShipId == membershipId);
            if (membership == null) return false;
            // Convert months to days (approximate)
            membership.MaxBorrowDurationDays = durationInDays;
            membership.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }
    }
}
