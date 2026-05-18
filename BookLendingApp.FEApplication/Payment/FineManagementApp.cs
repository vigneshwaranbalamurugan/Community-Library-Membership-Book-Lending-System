using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Security;
using BookLendingApp.FEApplication.Validation;
using ModelMember = BookLendingApp.ModelLibrary.Models.Member;
using ModelPayment = BookLendingApp.ModelLibrary.Models.Payment;

namespace BookLendingApp.Application.Payment
{
    public class FineManagementApp
    {
        private readonly IFineManagementService _fineManagementService;
        private readonly AppSession _session;

        public FineManagementApp(IFineManagementService fineManagementService, AppSession session)
        {
            _fineManagementService = fineManagementService;
            _session = session;
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

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 4);

                switch (choice)
                {
                    case 1: ViewPendingFines(); break;
                    case 2: PayFine(); break;
                    case 3: FineHistory(); break;
                    case 4: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void ViewPendingFines()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var pending = _fineManagementService.GetPendingFines(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (pending.Count == 0)
            {
                Console.WriteLine("No pending fines found.");
                return;
            }

            foreach (var payment in pending)
            {
                Console.WriteLine($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
            Console.WriteLine($"Total pending: ₹{_fineManagementService.GetPendingFineTotal(member.MemberId)}");
        }

        private void PayFine()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var pending = _fineManagementService.GetPendingFines(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (pending.Count == 0)
            {
                Console.WriteLine("No pending fines to pay.");
                return;
            }

            var selected = ConsoleInputValidator.PromptSelection<ModelPayment>(
                "Select a pending fine to pay:",
                pending,
                payment => $"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType}");

            var paymentId = selected.PaymentId;
            Console.WriteLine(_fineManagementService.PayFine(paymentId) ? "Fine paid." : "Payment not found.");
        }

        private void FineHistory()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var history = _fineManagementService.GetFineHistory(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (history.Count == 0)
            {
                Console.WriteLine("No fine history found.");
                return;
            }

            foreach (var payment in history)
            {
                Console.WriteLine($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
        }

        private bool TryGetCurrentMember(out ModelMember member)
        {
            member = _session.CurrentMember!;
            if (_session.IsMember && member != null)
            {
                return true;
            }

            Console.WriteLine("Please sign in as a user to access fine management.");
            return false;
        }
    }
}