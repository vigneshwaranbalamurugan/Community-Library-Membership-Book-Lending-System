using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Security;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;
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
                ConsoleUi.WriteTitle("Fine Menu");
                ConsoleUi.WriteMenuOptions(new[] { "View Pending Fines", "Pay Fine", "Fine History", "Back" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 4);

                switch (choice)
                {
                    case 1: ViewPendingFines(); break;
                    case 2: PayFine(); break;
                    case 3: FineHistory(); break;
                    case 4: return;
                    default: ConsoleUi.WriteError("Invalid choice."); ConsoleUi.Pause(); break;
                }
            }
        }

        private void ViewPendingFines()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var pending = _fineManagementService.GetPendingFines(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (pending.Count == 0)
            {
                ConsoleUi.WriteInfo("No pending fines found.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var payment in pending)
            {
                rows.Add($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.WriteInfo($"Total pending: ₹{_fineManagementService.GetPendingFineTotal(member.MemberId)}");
            ConsoleUi.Pause();
        }

        private void PayFine()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var pending = _fineManagementService.GetPendingFines(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (pending.Count == 0)
            {
                ConsoleUi.WriteInfo("No pending fines to pay.");
                ConsoleUi.Pause();
                return;
            }

            var selected = ConsoleInputValidator.PromptSelection<ModelPayment>(
                "Select a pending fine to pay:",
                pending,
                payment => $"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType}");

            var paymentId = selected.PaymentId;
            var ok = _fineManagementService.PayFine(paymentId);
            ConsoleUi.WriteInfo(ok ? "Fine paid." : "Payment not found.");
            ConsoleUi.Pause();
        }

        private void FineHistory()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var history = _fineManagementService.GetFineHistory(member.MemberId) ?? new System.Collections.Generic.List<ModelPayment>();
            if (history.Count == 0)
            {
                ConsoleUi.WriteInfo("No fine history found.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var payment in history)
            {
                rows.Add($"PaymentId: {payment.PaymentId} | Amount: {payment.Amount} | Type: {payment.PaymentType} | Paid: {payment.IsPaid}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private bool TryGetCurrentMember(out ModelMember member)
        {
            member = _session.CurrentMember!;
            if (_session.IsMember && member != null)
            {
                return true;
            }

            ConsoleUi.WriteInfo("Please sign in as a user to access fine management.");
            ConsoleUi.Pause();
            return false;
        }
    }
}