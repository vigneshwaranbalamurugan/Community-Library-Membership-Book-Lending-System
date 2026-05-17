using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IPaymentRepository : IRepository<Guid, Payment>
    {
        List<Payment> GetPaymentsByMember(Guid memberId);
        List<Payment> GetPaymentsByBorrowRecord(Guid borrowRecordId);
        List<Payment> GetUnpaidPaymentsByMember(Guid memberId);
        decimal GetUnpaidAmountByMember(Guid memberId);
        decimal GetTotalPaidByBorrowRecord(Guid borrowRecordId);
        bool MarkPaymentPaid(Guid paymentId);
    }
}