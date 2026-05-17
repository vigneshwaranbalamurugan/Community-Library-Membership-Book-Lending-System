using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IFineManagementService
    {
        List<Payment> GetPendingFines(Guid memberId);
        decimal GetPendingFineTotal(Guid memberId);
        bool PayFine(Guid paymentId);
        List<Payment> GetFineHistory(Guid memberId);
    }
}