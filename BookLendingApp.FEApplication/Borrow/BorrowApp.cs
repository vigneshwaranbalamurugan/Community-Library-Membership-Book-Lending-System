using System;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Application.Borrow
{
    public class BorrowApp
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowApp(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        public void BorrowMenu()
        {
            while (true)
            {
                Console.WriteLine("Borrow Menu:");
                Console.WriteLine("1. Borrow Book");
                Console.WriteLine("2. Return Book");
                Console.WriteLine("3. Renew Book");
                Console.WriteLine("4. Member Borrowing Summary");
                Console.WriteLine("5. Overdue Books");
                Console.WriteLine("6. Back");

                switch (Console.ReadLine())
                {
                    case "1": BorrowBook(); break;
                    case "2": ReturnBook(); break;
                    case "3": RenewBook(); break;
                    case "4": MemberSummary(); break;
                    case "5": OverdueBooks(); break;
                    case "6": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void BorrowBook()
        {
            Console.WriteLine("Enter member id:");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId)) return;
            Console.WriteLine("Enter book copy id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookCopyId)) return;

            try
            {
                var borrow = _borrowingService.BorrowBook(memberId, bookCopyId);
                Console.WriteLine($"Borrowed. BorrowRecordId: {borrow.BorrowRecordId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Borrow failed: {ex.Message}");
            }
        }

        private void ReturnBook()
        {
            Console.WriteLine("Enter borrow record id:");
            if (!Guid.TryParse(Console.ReadLine(), out var borrowRecordId)) return;
            Console.WriteLine("Enter return date (yyyy-MM-dd), or leave blank for today:");
            var dateInput = Console.ReadLine();
            var returnDate = string.IsNullOrWhiteSpace(dateInput) ? DateTime.UtcNow : DateTime.Parse(dateInput);

            try
            {
                _borrowingService.ReturnBook(borrowRecordId, returnDate);
                Console.WriteLine("Returned successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Return failed: {ex.Message}");
            }
        }

        private void RenewBook()
        {
            Console.WriteLine("Enter borrow record id:");
            if (!Guid.TryParse(Console.ReadLine(), out var borrowRecordId)) return;

            try
            {
                _borrowingService.RenewBorrow(borrowRecordId);
                Console.WriteLine("Renewed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Renew failed: {ex.Message}");
            }
        }

        private void MemberSummary()
        {
            Console.WriteLine("Enter member id:");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId)) return;
            var activeBorrows = _borrowingService.GetActiveBorrowRecords(memberId);
            Console.WriteLine($"Active borrows: {activeBorrows.Count}");
            Console.WriteLine($"Unpaid fine: ₹{_borrowingService.GetUnpaidFine(memberId)}");
        }

        private void OverdueBooks()
        {
            foreach (var record in _borrowingService.GetOverdueBorrowRecords())
            {
                Console.WriteLine($"BorrowRecordId: {record.BorrowRecordId} | MemberId: {record.MemberId} | BookCopyId: {record.BookCopyId}");
            }
        }
    }
}