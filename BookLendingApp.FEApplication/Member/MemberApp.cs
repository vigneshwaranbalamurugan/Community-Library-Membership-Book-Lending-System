using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

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
                ConsoleUi.WriteTitle("Member Menu");
                ConsoleUi.WriteMenuOptions(new[] {
                    "Add Member",
                    "View Members",
                    "Update Member",
                    "Delete Member",
                    "View Active Members",
                    "Update Membership",
                    "Update Status",
                    "Search by Email",
                    "Search by Phone",
                    "Back"
                });

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
                    default: ConsoleUi.WriteError("Invalid choice."); ConsoleUi.Pause(); break;
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
                ConsoleUi.WriteSuccess("Member added.");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error adding member: {ex.Message}");
                ConsoleUi.Pause();
            }
        }

        private void ViewMembers()
        {
            try
            {
                var members = _memberService.GetAllMembers();
                if (members.Count == 0)
                {
                    ConsoleUi.WriteInfo("No members found.");
                    ConsoleUi.Pause();
                    return;
                }

                var rows = new System.Collections.Generic.List<string>();
                foreach (var member in members)
                {
                    rows.Add($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId} | Active: {member.IsActive} | Mobile: {member.MobileNumber} | MembershipId: {member.MembershipId}");
                }
                ConsoleUi.WriteTable(rows);
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error retrieving members: {ex.Message}");
                ConsoleUi.Pause();
            }
        }

        private void UpdateMember()
        {
            var memberId = PromptMemberSelection();
            if (memberId == Guid.Empty)
            {
                return;
            }

            var existingMember = _memberService.GetMemberById(memberId);
            if (existingMember == null)
            {
                ConsoleUi.WriteError("Member not found.");
                return;
            }

            ConsoleUi.WriteInfo($"Current values: Name={existingMember.FullName}, Email={existingMember.EmailId}, Mobile={existingMember.MobileNumber}, MembershipId={existingMember.MembershipId}, Active={existingMember.IsActive}");

            var fullName = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter full name", existingMember.FullName);
            var email = ConsoleInputValidator.ReadEmailWithDefault("Enter email", existingMember.EmailId);

            var password = existingMember.Password;
            if (ConsoleInputValidator.ReadYesNo("Change password?", defaultValue: false))
            {
                password = ConsoleInputValidator.ReadRequiredString("Enter password:");
            }

            var membershipId = existingMember.MembershipId;
            if (ConsoleInputValidator.ReadYesNo($"Change membership? Current is '{existingMember.MembershipId}'", defaultValue: false))
            {
                membershipId = PromptMembershipSelection();
                if (membershipId == Guid.Empty)
                {
                    return;
                }
            }

            var isActive = ConsoleInputValidator.ReadYesNo($"Is active? Current is {(existingMember.IsActive ? "Yes" : "No")}", defaultValue: existingMember.IsActive);
            var mobile = ConsoleInputValidator.ReadOptionalMobileNumberWithDefault("Enter mobile number (optional)", existingMember.MobileNumber);

            try
            {
                _memberService.UpdateMember(memberId, fullName, email, password, membershipId, isActive, mobile);
                ConsoleUi.WriteSuccess("Member updated.");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error updating member: {ex.Message}");
                ConsoleUi.Pause();
            }
        }

        private void DeleteMember()
        {
            var memberId = PromptMemberSelection();

            try
            {
                _memberService.RemoveMember(memberId);
                ConsoleUi.WriteSuccess("Member deleted.");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Cannot delete member: {ex.Message}");
                ConsoleUi.WriteInfo("Note: Ensure the member has no active borrows, pending fines, or history.");
            }
            ConsoleUi.Pause();
        }

        private void ViewActiveMembers()
        {
            var members = _memberService.GetActiveMembers();
            if (members == null || members.Count == 0)
            {
                ConsoleUi.WriteInfo("No active members found.");
                ConsoleUi.Pause();
                return;
            }
            var rows = new System.Collections.Generic.List<string>();
            foreach (var member in members)
            {
                rows.Add($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void UpdateMembership()
        {
            var memberId = PromptMemberSelection();
            if (memberId == Guid.Empty) return;

            var member = _memberService.GetMemberById(memberId);
            if (member == null)
            {
                ConsoleUi.WriteError("Member not found.");
                return;
            }

            ConsoleUi.WriteInfo($"Current MembershipId: {member.MembershipId}");

            if (!ConsoleInputValidator.ReadYesNo("Change membership?", defaultValue: false))
            {
                ConsoleUi.WriteInfo("No changes made.");
                ConsoleUi.Pause();
                return;
            }

            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty)
            {
                return;
            }

            var ok = _memberService.UpdateMembership(memberId, membershipId.ToString());
            ConsoleUi.WriteInfo(ok ? "Membership updated." : "Membership update failed.");
            ConsoleUi.Pause();
        }

        private void UpdateStatus()
        {
            var memberId = PromptMemberSelection();
            if (memberId == Guid.Empty)
            {
                return;
            }

            var member = _memberService.GetMemberById(memberId);
            if (member == null)
            {
                ConsoleUi.WriteError("Member not found.");
                return;
            }

            var isActive = ConsoleInputValidator.ReadYesNo($"Set member as active? Current is {(member.IsActive ? "Yes" : "No")}", defaultValue: member.IsActive);
            var ok = _memberService.UpdateStatus(memberId, isActive);
            ConsoleUi.WriteInfo(ok ? "Status updated." : "Status update failed.");
            ConsoleUi.Pause();
        }

        private void SearchByEmail()
        {
            var email = ConsoleInputValidator.ReadEmail("Enter email:");
            try
            {
                var member = _memberService.GetMemberByEmail(email);
                if (member == null)
                {
                    ConsoleUi.WriteInfo("No member found with that email.");
                    ConsoleUi.Pause();
                    return;
                }

                ConsoleUi.WriteInfo($"ID: {member.MemberId} | Name: {member.FullName} | Email: {member.EmailId}");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error: {ex.Message}");
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
                    ConsoleUi.WriteInfo("No member found with that phone.");
                    ConsoleUi.Pause();
                    return;
                }

                ConsoleUi.WriteInfo($"ID: {member.MemberId} | Name: {member.FullName} | Phone: {member.MobileNumber}");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error: {ex.Message}");
            }
        }

        private Guid PromptMembershipSelection()
        {
            var memberships = _membershipService.GetAllMemberships();
            if (memberships.Count == 0)
            {
                ConsoleUi.WriteInfo("No membership types found. Create one first.");
                ConsoleUi.Pause();
                return Guid.Empty;
            }

            var selectedMembership = ConsoleInputValidator.PromptSelection(
                "Choose a membership type:",
                memberships,
                membership => $"Name: {membership.Name} | Books: {membership.MaxBooksAllowed} | Days: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")}");

            return selectedMembership.MemberShipId;
        }

        private Guid PromptMemberSelection()
        {
            var members = _memberService.GetAllMembers();
            if (members.Count == 0)
            {
                ConsoleUi.WriteInfo("No members found. Add a member first.");
                ConsoleUi.Pause();
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a member:",
                members,
                m => $"Name: {m.FullName} | Email: {m.EmailId} | Active: {m.IsActive}");

            return selected.MemberId;
        }
    }
}