using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

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
            ConsoleUi.WriteTitle("Fine Rules");
            ConsoleUi.WriteMenuOptions(new[] { "Add new rule", "List rules", "Edit rule", "Remove rule", "Filter by type", "Back" });

            var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 6);

                switch (choice)
                {
                    case 1: AddFineRule(); break;
                    case 2: ViewFineRules(); break;
                    case 3: UpdateFineRule(); break;
                    case 4: DeleteFineRule(); break;
                    case 5: ViewByFineType(); break;
                    case 6: return;
                    default: ConsoleUi.WriteError("Invalid choice."); break;
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
            ConsoleUi.WriteSuccess("Fine rule added.");
            ConsoleUi.Pause();
        }

        private void ViewFineRules()
        {
            var rules = _fineRuleService.GetAllFineRules() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.FineRule>();
            if (rules.Count == 0)
            {
                ConsoleUi.WriteInfo("No fine rules found.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var rule in rules)
            {
                rows.Add($"ID: {rule.FineAmountId} | Name: {rule.Name} | Type: {rule.FineType} | Amount: {rule.Amount}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void UpdateFineRule()
        {
            var fineRuleId = PromptFineRuleSelection();
            if (fineRuleId == Guid.Empty)
            {
                return;
            }

            var existing = _fineRuleService.GetFineRuleById(fineRuleId);
            if (existing == null)
            {
                ConsoleUi.WriteError("Fine rule not found.");
                return;
            }

            ConsoleUi.WriteInfo($"Current values: Type={existing.FineType}, Name={existing.Name}, CalcType={existing.FineCalculationType}, Amount={existing.Amount}, Description={existing.Description}, Percentage={existing.Percentage}");

            var fineType = ConsoleInputValidator.PromptEnumSelectionWithDefault("Select a fine type", existing.FineType);
            var name = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter name", existing.Name);
            var calculationType = ConsoleInputValidator.PromptEnumSelectionWithDefault("Select a calculation type", existing.FineCalculationType);
            var amount = ConsoleInputValidator.ReadDecimalWithDefault("Enter amount", existing.Amount, 0m);
            var description = ConsoleInputValidator.ReadOptionalStringWithDefault("Enter description (optional)", existing.Description);
            var percentage = ConsoleInputValidator.ReadOptionalStringWithDefault("Enter percentage (optional, type '-' to clear)", existing.Percentage?.ToString());
            decimal? percentageValue = null;
            if (!string.IsNullOrWhiteSpace(percentage))
            {
                percentageValue = decimal.Parse(percentage, System.Globalization.CultureInfo.InvariantCulture);
            }

            _fineRuleService.UpdateFineRule(fineRuleId, fineType, name, calculationType, amount, description, percentageValue);
            ConsoleUi.WriteSuccess("Fine rule updated.");
            ConsoleUi.Pause();
        }

        private void DeleteFineRule()
        {
            var fineRuleId = PromptFineRuleSelection();
            if (fineRuleId == Guid.Empty) return;

            try
            {
                _fineRuleService.RemoveFineRule(fineRuleId);
                ConsoleUi.WriteSuccess("Fine rule deleted.");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Cannot delete fine rule: {ex.Message}");
                ConsoleUi.WriteInfo("Note: This rule might be required for existing fine calculations.");
            }
            ConsoleUi.Pause();
        }

        private void ViewByFineType()
        {
            var fineType = ConsoleInputValidator.PromptEnumSelection<FineType>("Select a fine type:");
            var list = _fineRuleService.GetFineRulesByFineType(fineType);
            var rows = new System.Collections.Generic.List<string>();
            foreach (var rule in list)
            {
                rows.Add($"ID: {rule.FineAmountId} | Name: {rule.Name} | Amount: {rule.Amount}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private Guid PromptFineRuleSelection()
        {
            var rules = _fineRuleService.GetAllFineRules();
            if (rules.Count == 0)
            {
                ConsoleUi.WriteInfo("No fine rules found.");
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a fine rule:",
                rules,
                r => $"Name: {r.Name} | Type: {r.FineType} | Amount: {r.Amount}");

            return selected.FineAmountId;
        }
    }
}