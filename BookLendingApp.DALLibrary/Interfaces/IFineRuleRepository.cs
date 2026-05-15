using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IFineRuleRepository : IRepository<Guid, FineRule>
    {
        List<FineRule> GetFineRulesByFineType(FineType fineType);

    }
}