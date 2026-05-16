using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Services
{
    public class FineRuleService : IFineRuleService
    {
        private readonly IFineRuleRepository _fineRuleRepository;

        public FineRuleService(IFineRuleRepository fineRuleRepository)
        {
            _fineRuleRepository = fineRuleRepository;
        }

        public FineRule AddFineRule(FineType fineType, string name, FineCalculationType fineCalculationType, decimal amount, string? description = null, decimal? percentage = null)
        {
            var rule = new FineRule
            {
                FineType = fineType,
                Name = name,
                FineCalculationType = fineCalculationType,
                Amount = amount,
                Description = description,
                Percentage = percentage
            };
            return _fineRuleRepository.Create(rule);
        }

        public FineRule UpdateFineRule(Guid fineRuleId, FineType fineType, string name, FineCalculationType fineCalculationType, decimal amount, string? description = null, decimal? percentage = null)
        {
            var updated = new FineRule
            {
                FineAmountId = fineRuleId,
                FineType = fineType,
                Name = name,
                FineCalculationType = fineCalculationType,
                Amount = amount,
                Description = description,
                Percentage = percentage
            };
            return _fineRuleRepository.Update(fineRuleId, updated);
        }

        public void RemoveFineRule(Guid fineRuleId) => _fineRuleRepository.Delete(fineRuleId);

        public FineRule GetFineRuleById(Guid fineRuleId)
        {
            return _fineRuleRepository.Get(fineRuleId) ?? throw new KeyNotFoundException($"FineRule {fineRuleId} not found.");
        }

        public List<FineRule> GetAllFineRules() => _fineRuleRepository.GetAll();

        public List<FineRule> GetFineRulesByFineType(FineType fineType) => _fineRuleRepository.GetFineRulesByFineType(fineType);
    }
}