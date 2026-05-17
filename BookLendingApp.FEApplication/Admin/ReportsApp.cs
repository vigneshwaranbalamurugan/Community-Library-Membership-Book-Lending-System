using System;
using BookLendingApp.Ballibrary.Interfaces;

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
                Console.WriteLine("Reports Menu:");
                Console.WriteLine("1. Overdue Books");
                Console.WriteLine("2. Back");

                switch (Console.ReadLine())
                {
                    case "1": ShowOverdueBooks(); break;
                    case "2": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void ShowOverdueBooks()
        {
            foreach (var record in _borrowingService.GetOverdueBorrowRecords())
            {
                Console.WriteLine($"BorrowRecordId: {record.BorrowRecordId} | MemberId: {record.MemberId} | BookCopyId: {record.BookCopyId}");
            }
        }
    }
}