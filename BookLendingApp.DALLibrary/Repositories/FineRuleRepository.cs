using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.ModelLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class FineRuleRepository: AbstractRepository<Guid, FineRule>,IFineRuleRepository
    {
        public FineRuleRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override FineRule Create(FineRule item)
        {
            _context.FineRules.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override FineRule Update(Guid key, FineRule item)
        {
            var existingRule = _context.FineRules.FirstOrDefault(f => f.FineAmountId == key);
            if (existingRule == null)
                throw new KeyNotFoundException($"FineRule with ID {key} not found.");
            
            existingRule.FineType = item.FineType;
            existingRule.Name = item.Name;
            existingRule.Description = item.Description;
            existingRule.FineCalculationType = item.FineCalculationType;
            existingRule.Percentage = item.Percentage;
            existingRule.Amount = item.Amount;
            existingRule.UpdatedAt = DateTime.UtcNow;
            
            _context.SaveChanges();
            return existingRule;
        }
        public override FineRule? Get(Guid key)
        {
            return _context.FineRules.FirstOrDefault(f => f.FineAmountId == key);
        }
        public override List<FineRule> GetAll()
        {
            return _context.FineRules.ToList();
        }
        public override FineRule Delete(Guid key)
        {
            var rule = _context.FineRules.FirstOrDefault(f => f.FineAmountId == key);
            if (rule == null)
                throw new KeyNotFoundException($"FineRule with ID {key} not found.");
            
            _context.FineRules.Remove(rule);
            _context.SaveChanges();
            return rule;
        }

        public List<FineRule> GetFineRulesByFineType(FineType fineType)
        {
            return _context.FineRules.Where(f => f.FineType == fineType).ToList();
        }
    }
}
