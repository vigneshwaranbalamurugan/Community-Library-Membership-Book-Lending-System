using System;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Application.Member
{
    public class MemberApp
    {
        private readonly IMemberService _memberService;

        public MemberApp(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public void MemberMenu()
        {
            while (true)
            {
                Console.WriteLine("Member Menu:");
                Console.WriteLine("1. Add Member");
                Console.WriteLine("2. View Members");
                Console.WriteLine("3. Update Member");
                Console.WriteLine("4. Delete Member");
                Console.WriteLine("5. View Active Members");
                Console.WriteLine("6. Update Membership");
                Console.WriteLine("7. Update Status");
                Console.WriteLine("8. Search by Email");
                Console.WriteLine("9. Search by Phone");
                Console.WriteLine("10. Back");

                switch (Console.ReadLine())
                {
                    case "1": AddMember(); break;
                    case "2": ViewMembers(); break;
                    case "3": UpdateMember(); break;
                    case "4": DeleteMember(); break;
                    case "5": ViewActiveMembers(); break;
                    case "6": UpdateMembership(); break;
                    case "7": UpdateStatus(); break;
                    case "8": SearchByEmail(); break;
                    case "9": SearchByPhone(); break;
                    case "10": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddMember()
        {
            Console.WriteLine("Enter full name:");
            var fullName = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter membership id:");
            var membershipIdInput = Console.ReadLine();
            if (!Guid.TryParse(membershipIdInput, out var membershipId))
            {
                Console.WriteLine("Invalid membership id.");
                return;
            }
            Console.WriteLine("Enter mobile number (optional):");
            var mobile = Console.ReadLine();

            try
            {
                _memberService.AddMember(fullName, email, password, membershipId, mobile);
                Console.WriteLine("Member added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding member: {ex.Message}");
            }
        }

        private void ViewMembers()
        {
            try
            {
                var members = _memberService.GetAllMembers();
                if (members.Count == 0)
                {
                    Console.WriteLine("No members found.");
                    return;
                }

                foreach (var member in members)
                {
                    Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId} | Active: {member.IsActive} | Mobile: {member.MobileNumber} | MembershipId: {member.MembershipId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving members: {ex.Message}");
            }
        }

        private void UpdateMember()
        {
            Console.WriteLine("Enter member id:");
            var memberIdInput = Console.ReadLine();
            if (!Guid.TryParse(memberIdInput, out var memberId))
            {
                Console.WriteLine("Invalid member id.");
                return;
            }

            Console.WriteLine("Enter full name:");
            var fullName = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter membership id:");
            var membershipIdInput = Console.ReadLine();
            if (!Guid.TryParse(membershipIdInput, out var membershipId))
            {
                Console.WriteLine("Invalid membership id.");
                return;
            }

            Console.WriteLine("Is active? (true/false):");
            var isActiveInput = Console.ReadLine();
            bool.TryParse(isActiveInput, out var isActive);
            Console.WriteLine("Enter mobile number (optional):");
            var mobile = Console.ReadLine();

            try
            {
                _memberService.UpdateMember(memberId, fullName, email, password, membershipId, isActive, mobile);
                Console.WriteLine("Member updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating member: {ex.Message}");
            }
        }

        private void DeleteMember()
        {
            Console.WriteLine("Enter member id:");
            var memberIdInput = Console.ReadLine();
            if (!Guid.TryParse(memberIdInput, out var memberId))
            {
                Console.WriteLine("Invalid member id.");
                return;
            }

            try
            {
                _memberService.RemoveMember(memberId);
                Console.WriteLine("Member deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting member: {ex.Message}");
            }
        }

        private void ViewActiveMembers()
        {
            var members = _memberService.GetActiveMembers();
            foreach (var member in members)
            {
                Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId}");
            }
        }

        private void UpdateMembership()
        {
            Console.WriteLine("Enter member id:");
            var memberIdInput = Console.ReadLine();
            if (!Guid.TryParse(memberIdInput, out var memberId))
            {
                Console.WriteLine("Invalid member id.");
                return;
            }

            Console.WriteLine("Enter new membership id:");
            var membershipId = Console.ReadLine() ?? string.Empty;
            var ok = _memberService.UpdateMembership(memberId, membershipId);
            Console.WriteLine(ok ? "Membership updated." : "Membership update failed.");
        }

        private void UpdateStatus()
        {
            Console.WriteLine("Enter member id:");
            var memberIdInput = Console.ReadLine();
            if (!Guid.TryParse(memberIdInput, out var memberId))
            {
                Console.WriteLine("Invalid member id.");
                return;
            }

            Console.WriteLine("Enter status (true/false):");
            var isActiveInput = Console.ReadLine();
            bool.TryParse(isActiveInput, out var isActive);
            var ok = _memberService.UpdateStatus(memberId, isActive);
            Console.WriteLine(ok ? "Status updated." : "Status update failed.");
        }

        private void SearchByEmail()
        {
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine() ?? string.Empty;
            try
            {
                var member = _memberService.GetMemberByEmail(email);
                Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SearchByPhone()
        {
            Console.WriteLine("Enter phone:");
            var phone = Console.ReadLine() ?? string.Empty;
            try
            {
                var member = _memberService.GetMemberByPhone(phone);
                Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Phone: {member.MobileNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}