using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Services
{
    public class FineManagementService : IFineManagementService
    {
        private readonly IPaymentRepository _paymentRepository;

        public FineManagementService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public List<Payment> GetPendingFines(Guid memberId) => _paymentRepository.GetUnpaidPaymentsByMember(memberId);

        public decimal GetPendingFineTotal(Guid memberId) => _paymentRepository.GetUnpaidAmountByMember(memberId);

        public bool PayFine(Guid paymentId) => _paymentRepository.MarkPaymentPaid(paymentId);

        public List<Payment> GetFineHistory(Guid memberId) => _paymentRepository.GetPaymentsByMember(memberId);
    }
}