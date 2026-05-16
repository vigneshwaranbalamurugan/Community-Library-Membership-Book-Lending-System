using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IFineRuleService
    {
        FineRule AddFineRule(FineType fineType, string name, FineCalculationType fineCalculationType, decimal amount, string? description = null, decimal? percentage = null);
        FineRule UpdateFineRule(Guid fineRuleId, FineType fineType, string name, FineCalculationType fineCalculationType, decimal amount, string? description = null, decimal? percentage = null);
        void RemoveFineRule(Guid fineRuleId);
        FineRule GetFineRuleById(Guid fineRuleId);
        List<FineRule> GetAllFineRules();
        List<FineRule> GetFineRulesByFineType(FineType fineType);
    }
}