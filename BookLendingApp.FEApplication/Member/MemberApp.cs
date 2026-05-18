using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;

namespace BookLendingApp.Application.Member
{
    public class MemberApp
    {
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;

        public MemberApp(IMemberService memberService, IMembershipService membershipService)
        {
            _memberService = memberService;
            _membershipService = membershipService;
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

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 10);

                switch (choice)
                {
                    case 1: AddMember(); break;
                    case 2: ViewMembers(); break;
                    case 3: UpdateMember(); break;
                    case 4: DeleteMember(); break;
                    case 5: ViewActiveMembers(); break;
                    case 6: UpdateMembership(); break;
                    case 7: UpdateStatus(); break;
                    case 8: SearchByEmail(); break;
                    case 9: SearchByPhone(); break;
                    case 10: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddMember()
        {
            var fullName = ConsoleInputValidator.ReadRequiredString("Enter full name:");
            var email = ConsoleInputValidator.ReadEmail("Enter email:");
            var password = ConsoleInputValidator.ReadRequiredString("Enter password:");
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty)
            {
                return;
            }
            var mobile = ConsoleInputValidator.ReadOptionalMobileNumber("Enter mobile number (optional):");

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
            var memberId = PromptMemberSelection();

            var fullName = ConsoleInputValidator.ReadRequiredString("Enter full name:");
            var email = ConsoleInputValidator.ReadEmail("Enter email:");
            var password = ConsoleInputValidator.ReadRequiredString("Enter password:");
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty)
            {
                return;
            }

            var isActive = ConsoleInputValidator.ReadYesNo("Is active?", defaultValue: true);
            var mobile = ConsoleInputValidator.ReadOptionalMobileNumber("Enter mobile number (optional):");

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
            var memberId = PromptMemberSelection();

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
            var memberId = PromptMemberSelection();
            if (memberId == Guid.Empty) return;

            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty)
            {
                return;
            }

            var ok = _memberService.UpdateMembership(memberId, membershipId.ToString());
            Console.WriteLine(ok ? "Membership updated." : "Membership update failed.");
        }

        private void UpdateStatus()
        {
            var memberId = PromptMemberSelection();

            var isActive = ConsoleInputValidator.ReadYesNo("Set member as active?", defaultValue: true);
            var ok = _memberService.UpdateStatus(memberId, isActive);
            Console.WriteLine(ok ? "Status updated." : "Status update failed.");
        }

        private void SearchByEmail()
        {
            var email = ConsoleInputValidator.ReadEmail("Enter email:");
            try
            {
                var member = _memberService.GetMemberByEmail(email);
                if (member == null)
                {
                    Console.WriteLine("No member found with that email.");
                    return;
                }

                Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SearchByPhone()
        {
            var phone = ConsoleInputValidator.ReadMobileNumber("Enter phone:");
            try
            {
                var member = _memberService.GetMemberByPhone(phone);
                if (member == null)
                {
                    Console.WriteLine("No member found with that phone.");
                    return;
                }

                Console.WriteLine($"ID: {member.MemberId} | Name: {member.FullName} | Phone: {member.MobileNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private Guid PromptMembershipSelection()
        {
            var memberships = _membershipService.GetAllMemberships();
            if (memberships.Count == 0)
            {
                Console.WriteLine("No membership types found. Create one first.");
                return Guid.Empty;
            }

            var selectedMembership = ConsoleInputValidator.PromptSelection(
                "Choose a membership type:",
                memberships,
                membership => $"{membership.Name} | Books: {membership.MaxBooksAllowed} | Days: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")}");

            return selectedMembership.MemberShipId;
        }

        private Guid PromptMemberSelection()
        {
            var members = _memberService.GetAllMembers();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found. Add a member first.");
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a member:",
                members,
                m => $"{m.FullName} | {m.EmailId} | Active: {m.IsActive}");

            return selected.MemberId;
        }
    }
}