using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.FEApplication.Validation;

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
                Console.WriteLine("Fine Rules:");
                Console.WriteLine("1. Add new rule");
                Console.WriteLine("2. List rules");
                Console.WriteLine("3. Edit rule");
                Console.WriteLine("4. Remove rule");
                Console.WriteLine("5. Filter by type");
                Console.WriteLine("6. Back");

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 6);

                switch (choice)
                {
                    case 1: AddFineRule(); break;
                    case 2: ViewFineRules(); break;
                    case 3: UpdateFineRule(); break;
                    case 4: DeleteFineRule(); break;
                    case 5: ViewByFineType(); break;
                    case 6: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddFineRule()
        {
            var fineType = ConsoleInputValidator.PromptEnumSelection<FineType>("Select a fine type:");
            var name = ConsoleInputValidator.ReadRequiredString("Enter name:");
            var calculationType = ConsoleInputValidator.PromptEnumSelection<FineCalculationType>("Select a calculation type:");
            var amount = ConsoleInputValidator.ReadDecimal("Enter amount:", 0m);
            var description = ConsoleInputValidator.ReadOptionalString("Enter description (optional):");
            var percentage = ConsoleInputValidator.ReadOptionalString("Enter percentage (optional, leave blank if none):");
            decimal? percentageValue = null;
            if (!string.IsNullOrWhiteSpace(percentage))
            {
                percentageValue = decimal.Parse(percentage);
            }

            _fineRuleService.AddFineRule(fineType, name, calculationType, amount, description, percentageValue);
            Console.WriteLine("Fine rule added.");
        }

        private void ViewFineRules()
        {
            var rules = _fineRuleService.GetAllFineRules() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.FineRule>();
            if (rules.Count == 0)
            {
                Console.WriteLine("No fine rules found.");
                return;
            }

            foreach (var rule in rules)
            {
                Console.WriteLine($"ID: {rule.FineAmountId} | {rule.Name} | Type: {rule.FineType} | Amount: {rule.Amount}");
            }
        }

        private void UpdateFineRule()
        {
            var fineRuleId = PromptFineRuleSelection();
            var fineType = ConsoleInputValidator.PromptEnumSelection<FineType>("Select a fine type:");
            var name = ConsoleInputValidator.ReadRequiredString("Enter name:");
            var calculationType = ConsoleInputValidator.PromptEnumSelection<FineCalculationType>("Select a calculation type:");
            var amount = ConsoleInputValidator.ReadDecimal("Enter amount:", 0m);
            var description = ConsoleInputValidator.ReadOptionalString("Enter description (optional):");
            var percentage = ConsoleInputValidator.ReadOptionalString("Enter percentage (optional, leave blank if none):");
            decimal? percentageValue = null;
            if (!string.IsNullOrWhiteSpace(percentage))
            {
                percentageValue = decimal.Parse(percentage);
            }

            _fineRuleService.UpdateFineRule(fineRuleId, fineType, name, calculationType, amount, description, percentageValue);
            Console.WriteLine("Fine rule updated.");
        }

        private void DeleteFineRule()
        {
            var fineRuleId = PromptFineRuleSelection();
            if (fineRuleId == Guid.Empty) return;

            _fineRuleService.RemoveFineRule(fineRuleId);
            Console.WriteLine("Fine rule deleted.");
        }

        private void ViewByFineType()
        {
            var fineType = ConsoleInputValidator.PromptEnumSelection<FineType>("Select a fine type:");
            foreach (var rule in _fineRuleService.GetFineRulesByFineType(fineType))
            {
                Console.WriteLine($"ID: {rule.FineAmountId} | Name: {rule.Name} | Amount: {rule.Amount}");
            }
        }

        private Guid PromptFineRuleSelection()
        {
            var rules = _fineRuleService.GetAllFineRules();
            if (rules.Count == 0)
            {
                Console.WriteLine("No fine rules found.");
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a fine rule:",
                rules,
                r => $"{r.Name} | {r.FineType} | Amount: {r.Amount}");

            return selected.FineAmountId;
        }
    }
}