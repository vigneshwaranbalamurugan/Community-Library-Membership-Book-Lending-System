using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.Application.Payment
{
    public class FineRuleApp
    {
        private readonly IFineRuleService _fineRuleService;

        public FineRuleApp(IFineRuleService fineRuleService)
        {
            _fineRuleService = fineRuleService;
        }

        public void FineRuleMenu()
        {
            while (true)
            {
                Console.WriteLine("Fine Rule Menu:");
                Console.WriteLine("1. Add Fine Rule");
                Console.WriteLine("2. View Fine Rules");
                Console.WriteLine("3. Update Fine Rule");
                Console.WriteLine("4. Delete Fine Rule");
                Console.WriteLine("5. View By Fine Type");
                Console.WriteLine("6. Back");

                switch (Console.ReadLine())
                {
                    case "1": AddFineRule(); break;
                    case "2": ViewFineRules(); break;
                    case "3": UpdateFineRule(); break;
                    case "4": DeleteFineRule(); break;
                    case "5": ViewByFineType(); break;
                    case "6": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddFineRule()
        {
            Console.WriteLine("Enter fine type:");
            if (!Enum.TryParse(Console.ReadLine(), true, out FineType fineType)) return;
            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter calculation type:");
            if (!Enum.TryParse(Console.ReadLine(), true, out FineCalculationType calculationType)) return;
            Console.WriteLine("Enter amount:");
            decimal.TryParse(Console.ReadLine(), out var amount);
            Console.WriteLine("Enter description (optional):");
            var description = Console.ReadLine();
            Console.WriteLine("Enter percentage (optional):");
            decimal.TryParse(Console.ReadLine(), out var percentage);

            _fineRuleService.AddFineRule(fineType, name, calculationType, amount, description, percentage);
            Console.WriteLine("Fine rule added.");
        }

        private void ViewFineRules()
        {
            foreach (var rule in _fineRuleService.GetAllFineRules())
            {
                Console.WriteLine($"ID: {rule.FineAmountId} | Type: {rule.FineType} | Name: {rule.Name} | Amount: {rule.Amount} | Percentage: {rule.Percentage}");
            }
        }

        private void UpdateFineRule()
        {
            Console.WriteLine("Enter fine rule id:");
            if (!Guid.TryParse(Console.ReadLine(), out var fineRuleId)) return;
            Console.WriteLine("Enter fine type:");
            if (!Enum.TryParse(Console.ReadLine(), true, out FineType fineType)) return;
            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter calculation type:");
            if (!Enum.TryParse(Console.ReadLine(), true, out FineCalculationType calculationType)) return;
            Console.WriteLine("Enter amount:");
            decimal.TryParse(Console.ReadLine(), out var amount);
            Console.WriteLine("Enter description (optional):");
            var description = Console.ReadLine();
            Console.WriteLine("Enter percentage (optional):");
            decimal.TryParse(Console.ReadLine(), out var percentage);

            _fineRuleService.UpdateFineRule(fineRuleId, fineType, name, calculationType, amount, description, percentage);
            Console.WriteLine("Fine rule updated.");
        }

        private void DeleteFineRule()
        {
            Console.WriteLine("Enter fine rule id:");
            if (!Guid.TryParse(Console.ReadLine(), out var fineRuleId)) return;
            _fineRuleService.RemoveFineRule(fineRuleId);
            Console.WriteLine("Fine rule deleted.");
        }

        private void ViewByFineType()
        {
            Console.WriteLine("Enter fine type:");
            if (!Enum.TryParse(Console.ReadLine(), true, out FineType fineType)) return;
            foreach (var rule in _fineRuleService.GetFineRulesByFineType(fineType))
            {
                Console.WriteLine($"ID: {rule.FineAmountId} | Name: {rule.Name} | Amount: {rule.Amount}");
            }
        }
    }
}