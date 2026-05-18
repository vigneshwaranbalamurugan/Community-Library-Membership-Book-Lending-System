using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;

namespace BookLendingApp.Application.Admin
{
    public class ReportsApp
    {
        private readonly IBorrowingService _borrowingService;

        public ReportsApp(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        public void ReportsMenu()
        {
            while (true)
            {
                Console.WriteLine("Reports:");
                Console.WriteLine("1. Overdue books");
                Console.WriteLine("2. Back");

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 2);

                switch (choice)
                {
                    case 1: ShowOverdueBooks(); break;
                    case 2: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void ShowOverdueBooks()
        {
            var overdueRecords = _borrowingService.GetOverdueBorrowRecords();
            if (overdueRecords.Count == 0)
            {
                Console.WriteLine("No overdue books found.");
                return;
            }

            foreach (var record in overdueRecords)
            {
                Console.WriteLine($"Borrow: {record.BorrowRecordId} | Member: {record.MemberId} | Copy: {record.BookCopyId}");
            }
        }
    }
}