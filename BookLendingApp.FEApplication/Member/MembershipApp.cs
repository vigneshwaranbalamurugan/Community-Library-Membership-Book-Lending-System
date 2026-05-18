using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;

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
                Console.WriteLine("Membership Menu:");
                Console.WriteLine("1. Add Membership");
                Console.WriteLine("2. View Memberships");
                Console.WriteLine("3. Update Membership");
                Console.WriteLine("4. Delete Membership");
                Console.WriteLine("5. Update Max Books");
                Console.WriteLine("6. Update Duration");
                Console.WriteLine("7. Back");

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
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddMembership()
        {
            var name = ConsoleInputValidator.ReadRequiredString("Enter name:");
            var maxBooks = ConsoleInputValidator.ReadInt("Enter max books allowed:", 0);
            var maxBorrowDays = ConsoleInputValidator.ReadInt("Enter max borrow duration days:", 0);
            var isRenewalAllowed = ConsoleInputValidator.ReadYesNo("Is renewal allowed?", defaultValue: false);
            var maxRenewalTimes = ConsoleInputValidator.ReadInt("Enter max renewal times:", 0);
            var maxRenewalDays = ConsoleInputValidator.ReadInt("Enter max renewal duration days:", 0);
            var fee = ConsoleInputValidator.ReadDecimal("Enter membership fee:", 0m);

            _membershipService.AddMembership(name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            Console.WriteLine("Membership added.");
        }

        private void ViewMemberships()
        {
            var memberships = _membershipService.GetAllMemberships() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.MemberShip>();
            if (memberships.Count == 0)
            {
                Console.WriteLine("No memberships found.");
                return;
            }

            foreach (var membership in memberships)
            {
                Console.WriteLine(
                    $"ID: {membership.MemberShipId} | Name: {membership.Name} | Books: {membership.MaxBooksAllowed} | BorrowDays: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")} | RenewalTimes: {membership.MaxRenewalTimes} | RenewalDays: {membership.MaxRenewalDurationDays} | Fee: {membership.MembershipFee}");
            }
        }

        private void UpdateMembership()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;

            var name = ConsoleInputValidator.ReadRequiredString("Enter name:");
            var maxBooks = ConsoleInputValidator.ReadInt("Enter max books allowed:", 0);
            var maxBorrowDays = ConsoleInputValidator.ReadInt("Enter max borrow duration days:", 0);
            var isRenewalAllowed = ConsoleInputValidator.ReadYesNo("Is renewal allowed?", defaultValue: false);
            var maxRenewalTimes = ConsoleInputValidator.ReadInt("Enter max renewal times:", 0);
            var maxRenewalDays = ConsoleInputValidator.ReadInt("Enter max renewal duration days:", 0);
            var fee = ConsoleInputValidator.ReadDecimal("Enter membership fee:", 0m);

            _membershipService.UpdateMembership(membershipId, name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            Console.WriteLine("Membership updated.");
        }

        private void DeleteMembership()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;
            _membershipService.RemoveMembership(membershipId);
            Console.WriteLine("Membership deleted.");
        }

        private void UpdateMaxBooks()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;
            var maxBooks = ConsoleInputValidator.ReadInt("Enter new max books:", 0);
            Console.WriteLine(_membershipService.UpdateMembershipMaxBooks(membershipId, maxBooks) ? "Updated." : "Update failed.");
        }

        private void UpdateDuration()
        {
            var membershipId = PromptMembershipSelection();
            if (membershipId == Guid.Empty) return;
            var duration = ConsoleInputValidator.ReadInt("Enter new duration days:", 0);
            Console.WriteLine(_membershipService.UpdateMembershipDuration(membershipId, duration) ? "Updated." : "Update failed.");
        }

        private Guid PromptMembershipSelection()
        {
            var memberships = _membershipService.GetAllMemberships() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.MemberShip>();
            if (memberships.Count == 0)
            {
                Console.WriteLine("No memberships found. Create one first.");
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a membership:",
                memberships,
                membership => $"{membership.Name} | Books: {membership.MaxBooksAllowed} | Days: {membership.MaxBorrowDurationDays} | Renewal: {(membership.IsRenewalAllowed ? "Yes" : "No")}");

            return selected.MemberShipId;
        }
    }
}