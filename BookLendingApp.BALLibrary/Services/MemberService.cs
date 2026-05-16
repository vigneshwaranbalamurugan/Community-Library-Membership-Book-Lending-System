using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public Member AddMember(string fullName, string emailId, string password, Guid membershipId, string? mobileNumber = null)
        {
            var member = new Member
            {
                FullName = fullName,
                EmailId = emailId,
                Password = password,
                MembershipId = membershipId,
                MobileNumber = mobileNumber
            };
            return _memberRepository.Create(member);
        }

        public Member UpdateMember(Guid memberId, string fullName, string emailId, string password, Guid membershipId, bool isActive, string? mobileNumber = null)
        {
            var updated = new Member
            {
                MemberId = memberId,
                FullName = fullName,
                EmailId = emailId,
                Password = password,
                MembershipId = membershipId,
                IsActive = isActive,
                MobileNumber = mobileNumber
            };
            return _memberRepository.Update(memberId, updated);
        }

        public void RemoveMember(Guid memberId) => _memberRepository.Delete(memberId);

        public Member GetMemberById(Guid memberId)
        {
            return _memberRepository.Get(memberId) ?? throw new KeyNotFoundException($"Member {memberId} not found.");
        }

        public List<Member> GetAllMembers() => _memberRepository.GetAll();

        public Member GetMemberByEmail(string email) => _memberRepository.GetMemberByEmail(email);

        public Member GetMemberByPhone(string phone) => _memberRepository.GetMemberByPhone(phone);

        public bool UpdateMembership(Guid memberId, string newMembershipId) => _memberRepository.UpdateMembership(memberId, newMembershipId);

        public bool UpdateStatus(Guid memberId, bool isActive) => _memberRepository.UpdateStatus(memberId, isActive);

        public List<Member> GetActiveMembers() => _memberRepository.GetActiveMembers();
    }
}