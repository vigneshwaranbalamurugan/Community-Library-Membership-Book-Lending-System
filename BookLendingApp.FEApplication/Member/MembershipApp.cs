using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

namespace BookLendingApp.Application.Member
{
    public class MembershipApp
    {
        private readonly IMembershipService _membershipService;

        public MembershipApp(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public void MembershipMenu()
        {
            while (true)
            {
                ConsoleUi.WriteTitle("Membership Menu");
                ConsoleUi.WriteMenuOptions(new[] {
                    "Add Membership",
                    "View Memberships",
                    "Update Membership",
                    "Delete Membership",
                    "Update Max Books",
                    "Update Duration",
                    "Back"
                });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 7);

                switch (choice)
                {
                    case 1: AddMembership(); break;
                    case 2: ViewMemberships(); break;
                    case 3: UpdateMembership(); break;
                    case 4: DeleteMembership(); break;
                    case 5: UpdateMaxBooks(); break;
                    case 6: UpdateDuration(); break;
                    case 7: return;
                    default: ConsoleUi.WriteError("Invalid choice."); break;
                }
            }
        }

        private void AddMembership()
        {
            ConsoleUi.WriteTitle("Add Membership");
            var name = ConsoleInputValidator.ReadRequiredString("Enter name:");
            var maxBooks = ConsoleInputValidator.ReadInt("Enter max books allowed:", 0);
            var maxBorrowDays = ConsoleInputValidator.ReadInt("Enter max borrow duration days:", 0);
            var isRenewalAllowed = ConsoleInputValidator.ReadYesNo("Is renewal allowed?", defaultValue: false);
            var maxRenewalTimes = ConsoleInputValidator.ReadInt("Enter max renewal times:", 0);
            var maxRenewalDays = ConsoleInputValidator.ReadInt("Enter max renewal duration days:", 0);
            var fee = ConsoleInputValidator.ReadDecimal("Enter membership fee:", 0m);

            _membershipService.AddMembership(name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            ConsoleUi.WriteSuccess("Membership added.");
            ConsoleUi.Pause();
        }

        private void ViewMemberships()
        {
            var memberships = _membershipService.GetAllMemberships() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.MemberShip>();
            if (memberships.Count == 0)
            {
                ConsoleUi.WriteInfo("No memberships found.");
                ConsoleUi.Pause();
                return;
            }
            var rows = new System.Collections.Generic.List<string>();
            foreach (var membership in memberships)
            {
                rows.Add($"ID: {membership.MemberShipId} | Name: {membership.Name} | Books: {membership.MaxBooksAllowed} | BorrowDays: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")} | RenewalTimes: {membership.MaxRenewalTimes} | RenewalDays: {membership.MaxRenewalDurationDays} | Fee: {membership.MembershipFee}");
            }

            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void UpdateMembership()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;

            var existing = _membershipService.GetMembershipById(membershipId);
            if (existing == null)
            {
                ConsoleUi.WriteError("Membership not found.");
                return;
            }

            ConsoleUi.WriteInfo($"Current values: Name={existing.Name}, MaxBooks={existing.MaxBooksAllowed}, BorrowDays={existing.MaxBorrowDurationDays}, RenewalAllowed={existing.IsRenewalAllowed}, RenewalTimes={existing.MaxRenewalTimes}, RenewalDays={existing.MaxRenewalDurationDays}, Fee={existing.MembershipFee}");

            var name = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter name", existing.Name);
            var maxBooks = ConsoleInputValidator.ReadIntWithDefault("Enter max books allowed", existing.MaxBooksAllowed, 0);
            var maxBorrowDays = ConsoleInputValidator.ReadIntWithDefault("Enter max borrow duration days", existing.MaxBorrowDurationDays, 0);
            var isRenewalAllowed = ConsoleInputValidator.ReadYesNo($"Is renewal allowed? Current is {(existing.IsRenewalAllowed ? "Yes" : "No")}", defaultValue: existing.IsRenewalAllowed);
            var maxRenewalTimes = ConsoleInputValidator.ReadIntWithDefault("Enter max renewal times", existing.MaxRenewalTimes, 0);
            var maxRenewalDays = ConsoleInputValidator.ReadIntWithDefault("Enter max renewal duration days", existing.MaxRenewalDurationDays, 0);
            var fee = ConsoleInputValidator.ReadDecimalWithDefault("Enter membership fee", existing.MembershipFee, 0m);

            _membershipService.UpdateMembership(membershipId, name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            ConsoleUi.WriteSuccess("Membership updated.");
            ConsoleUi.Pause();
        }

        private void DeleteMembership()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;

            try
            {
                _membershipService.RemoveMembership(membershipId);
                ConsoleUi.WriteSuccess("Membership deleted.");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Cannot delete membership: {ex.Message}");
                ConsoleUi.WriteInfo("Note: Ensure no members are currently assigned to this membership type.");
            }
            ConsoleUi.Pause();
        }

        private void UpdateMaxBooks()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;
            var existing = _membershipService.GetMembershipById(membershipId);
            if (existing == null)
            {
                ConsoleUi.WriteError("Membership not found.");
                return;
            }

            var maxBooks = ConsoleInputValidator.ReadIntWithDefault("Enter new max books", existing.MaxBooksAllowed, 0);
            ConsoleUi.WriteInfo(_membershipService.UpdateMembershipMaxBooks(membershipId, maxBooks) ? "Updated." : "Update failed.");
            ConsoleUi.Pause();
        }

        private void UpdateDuration()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;
            var existing = _membershipService.GetMembershipById(membershipId);
            if (existing == null)
            {
                ConsoleUi.WriteError("Membership not found.");
                return;
            }

            var duration = ConsoleInputValidator.ReadIntWithDefault("Enter new duration days", existing.MaxBorrowDurationDays, 0);
            ConsoleUi.WriteInfo(_membershipService.UpdateMembershipDuration(membershipId, duration) ? "Updated." : "Update failed.");
            ConsoleUi.Pause();
        }

        private Guid PromptMembershipSelection()
        {
            var memberships = _membershipService.GetAllMemberships() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.MemberShip>();
            if (memberships.Count == 0)
            {
                ConsoleUi.WriteInfo("No memberships found. Create one first.");
                ConsoleUi.Pause();
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a membership:",
                memberships,
                membership => $"Name: {membership.Name} | Books: {membership.MaxBooksAllowed} | Days: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")}");

            return selected.MemberShipId;
        }
    }
}