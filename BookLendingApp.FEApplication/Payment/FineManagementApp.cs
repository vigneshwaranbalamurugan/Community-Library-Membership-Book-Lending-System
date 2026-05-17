using System;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Application.Payment
{
    public class FineManagementApp
    {
        private readonly IFineManagementService _fineManagementService;

        public FineManagementApp(IFineManagementService fineManagementService)
        {
            _fineManagementService = fineManagementService;
        }

        public void FineMenu()
        {
            while (true)
            {
                Console.WriteLine("Fine Menu:");
                Console.WriteLine("1. View Pending Fines");
                Console.WriteLine("2. Pay Fine");
                Console.WriteLine("3. Fine History");
                Console.WriteLine("4. Back");

                switch (Console.ReadLine())
                {
                    case "1": ViewPendingFines(); break;
                    case "2": PayFine(); break;
                    case "3": FineHistory(); break;
                    case "4": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void ViewPendingFines()
        {
            Console.WriteLine("Enter member id:");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId)) return;
            var pending = _fineManagementService.GetPendingFines(memberId);
            foreach (var payment in pending)
            {
                Console.WriteLine($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
            Console.WriteLine($"Total pending: ₹{_fineManagementService.GetPendingFineTotal(memberId)}");
        }

        private void PayFine()
        {
            Console.WriteLine("Enter payment id:");
            if (!Guid.TryParse(Console.ReadLine(), out var paymentId)) return;
            Console.WriteLine(_fineManagementService.PayFine(paymentId) ? "Fine paid." : "Payment not found.");
        }

        private void FineHistory()
        {
            Console.WriteLine("Enter member id:");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId)) return;
            foreach (var payment in _fineManagementService.GetFineHistory(memberId))
            {
                Console.WriteLine($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
        }
    }
}