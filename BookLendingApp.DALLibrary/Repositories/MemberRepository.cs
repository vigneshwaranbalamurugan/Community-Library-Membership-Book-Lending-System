using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class MemberRepository: AbstractRepository<Guid, Member>,IMemberRepository
    {
        public MemberRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override Member Create(Member item)
        {
            _context.Members.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override Member Update(Guid key, Member item)
        {
            var existingMember = _context.Members.FirstOrDefault(m => m.MemberId == key);
            if (existingMember == null)
                throw new KeyNotFoundException($"Member with ID {key} not found.");
            
            existingMember.FullName = item.FullName;
            existingMember.EmailId = item.EmailId;
            existingMember.IsActive = item.IsActive;
            existingMember.MobileNumber = item.MobileNumber;
            existingMember.Password = item.Password;
            existingMember.MembershipId = item.MembershipId;
            
            _context.SaveChanges();
            return existingMember;
        }
        public override Member? Get(Guid key)
        {
            return _context.Members.FirstOrDefault(m => m.MemberId == key);
        }
        public override List<Member> GetAll()
        {
            return _context.Members.ToList();
        }
        public override Member Delete(Guid key)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId == key);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {key} not found.");
            
            _context.Members.Remove(member);
            _context.SaveChanges();
            return member;
        }

        public Member GetMemberByEmail(string email)
        {
            var member = _context.Members.FirstOrDefault(m => m.EmailId == email);
            if (member == null)
                throw new KeyNotFoundException($"Member with email {email} not found.");
            return member;
        }

        public Member GetMemberByPhone(string phone)
        {
            var member = _context.Members.FirstOrDefault(m => m.MobileNumber == phone);
            if (member == null)
                throw new KeyNotFoundException($"Member with phone {phone} not found.");
            return member;
        }

        public bool UpdateMembership(Guid memberId, string newMembershipId)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId == memberId);
            if (member == null) return false;
            if (!Guid.TryParse(newMembershipId, out var membershipGuid)) return false;
            member.MembershipId = membershipGuid;
            _context.SaveChanges();
            return true;
        }

        public bool UpdateStatus(Guid memberId, bool isActive)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId == memberId);
            if (member == null) return false;
            member.IsActive = isActive;
            _context.SaveChanges();
            return true;
        }

        public List<Member> GetActiveMembers()
        {
            return _context.Members.Where(m => m.IsActive).ToList();
        }
    }
}
