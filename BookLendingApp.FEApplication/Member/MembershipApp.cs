using System;
using BookLendingApp.Ballibrary.Interfaces;

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

                switch (Console.ReadLine())
                {
                    case "1": AddMembership(); break;
                    case "2": ViewMemberships(); break;
                    case "3": UpdateMembership(); break;
                    case "4": DeleteMembership(); break;
                    case "5": UpdateMaxBooks(); break;
                    case "6": UpdateDuration(); break;
                    case "7": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddMembership()
        {
            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter max books allowed:");
            int.TryParse(Console.ReadLine(), out var maxBooks);
            Console.WriteLine("Enter max borrow duration days:");
            int.TryParse(Console.ReadLine(), out var maxBorrowDays);
            Console.WriteLine("Is renewal allowed? (true/false):");
            bool.TryParse(Console.ReadLine(), out var isRenewalAllowed);
            Console.WriteLine("Enter max renewal times:");
            int.TryParse(Console.ReadLine(), out var maxRenewalTimes);
            Console.WriteLine("Enter max renewal duration days:");
            int.TryParse(Console.ReadLine(), out var maxRenewalDays);
            Console.WriteLine("Enter membership fee:");
            decimal.TryParse(Console.ReadLine(), out var fee);

            _membershipService.AddMembership(name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            Console.WriteLine("Membership added.");
        }

        private void ViewMemberships()
        {
            var memberships = _membershipService.GetAllMemberships();
            foreach (var membership in memberships)
            {
                Console.WriteLine($"ID: {membership.MemberShipId} | Name: {membership.Name} | MaxBooks: {membership.MaxBooksAllowed} | DurationDays: {membership.MaxBorrowDurationDays} | Fee: {membership.MembershipFee}");
            }
        }

        private void UpdateMembership()
        {
            Console.WriteLine("Enter membership id:");
            if (!Guid.TryParse(Console.ReadLine(), out var membershipId)) return;

            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter max books allowed:");
            int.TryParse(Console.ReadLine(), out var maxBooks);
            Console.WriteLine("Enter max borrow duration days:");
            int.TryParse(Console.ReadLine(), out var maxBorrowDays);
            Console.WriteLine("Is renewal allowed? (true/false):");
            bool.TryParse(Console.ReadLine(), out var isRenewalAllowed);
            Console.WriteLine("Enter max renewal times:");
            int.TryParse(Console.ReadLine(), out var maxRenewalTimes);
            Console.WriteLine("Enter max renewal duration days:");
            int.TryParse(Console.ReadLine(), out var maxRenewalDays);
            Console.WriteLine("Enter membership fee:");
            decimal.TryParse(Console.ReadLine(), out var fee);

            _membershipService.UpdateMembership(membershipId, name, maxBooks, maxBorrowDays, isRenewalAllowed, maxRenewalTimes, maxRenewalDays, fee);
            Console.WriteLine("Membership updated.");
        }

        private void DeleteMembership()
        {
            Console.WriteLine("Enter membership id:");
            if (!Guid.TryParse(Console.ReadLine(), out var membershipId)) return;
            _membershipService.RemoveMembership(membershipId);
            Console.WriteLine("Membership deleted.");
        }

        private void UpdateMaxBooks()
        {
            Console.WriteLine("Enter membership id:");
            if (!Guid.TryParse(Console.ReadLine(), out var membershipId)) return;
            Console.WriteLine("Enter new max books:");
            int.TryParse(Console.ReadLine(), out var maxBooks);
            Console.WriteLine(_membershipService.UpdateMembershipMaxBooks(membershipId, maxBooks) ? "Updated." : "Update failed.");
        }

        private void UpdateDuration()
        {
            Console.WriteLine("Enter membership id:");
            if (!Guid.TryParse(Console.ReadLine(), out var membershipId)) return;
            Console.WriteLine("Enter new duration days:");
            int.TryParse(Console.ReadLine(), out var duration);
            Console.WriteLine(_membershipService.UpdateMembershipDuration(membershipId, duration) ? "Updated." : "Update failed.");
        }
    }
}