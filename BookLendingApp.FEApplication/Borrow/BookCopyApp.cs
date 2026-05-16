using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.Application.Borrow
{
    public class BookCopyApp
    {
        private readonly IBookCopyService _bookCopyService;

        public BookCopyApp(IBookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }

        public void BookCopyMenu()
        {
            while (true)
            {
                Console.WriteLine("Book Copy Menu:");
                Console.WriteLine("1. Add Book Copy");
                Console.WriteLine("2. View Book Copies");
                Console.WriteLine("3. Update Book Copy");
                Console.WriteLine("4. Delete Book Copy");
                Console.WriteLine("5. View By Book Id");
                Console.WriteLine("6. View Available By Book Id");
                Console.WriteLine("7. Search By Status");
                Console.WriteLine("8. Search By Condition");
                Console.WriteLine("9. Back");

                switch (Console.ReadLine())
                {
                    case "1": AddBookCopy(); break;
                    case "2": ViewBookCopies(); break;
                    case "3": UpdateBookCopy(); break;
                    case "4": DeleteBookCopy(); break;
                    case "5": ViewByBookId(); break;
                    case "6": ViewAvailableByBookId(); break;
                    case "7": SearchByStatus(); break;
                    case "8": SearchByCondition(); break;
                    case "9": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddBookCopy()
        {
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            Console.WriteLine("Enter barcode:");
            var barcode = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter shelf location:");
            var shelfLocation = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter status (Available/Borrowed/Lost/Reserved):");
            Enum.TryParse(Console.ReadLine(), true, out BookStatus status);
            Console.WriteLine("Enter damage percentage:");
            decimal.TryParse(Console.ReadLine(), out var damagePercentage);
            Console.WriteLine("Enter condition (optional):");
            var condition = Console.ReadLine();

            _bookCopyService.AddBookCopy(bookId, barcode, shelfLocation, status, damagePercentage, condition);
            Console.WriteLine("Book copy added.");
        }

        private void ViewBookCopies()
        {
            var copies = _bookCopyService.GetAllBookCopies();
            foreach (var copy in copies)
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | BookId: {copy.BookId} | Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status} | Condition: {copy.Condition}");
            }
        }

        private void UpdateBookCopy()
        {
            Console.WriteLine("Enter book copy id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookCopyId)) return;
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            Console.WriteLine("Enter barcode:");
            var barcode = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter shelf location:");
            var shelfLocation = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Enter status (Available/Borrowed/Lost/Reserved):");
            Enum.TryParse(Console.ReadLine(), true, out BookStatus status);
            Console.WriteLine("Enter damage percentage:");
            decimal.TryParse(Console.ReadLine(), out var damagePercentage);
            Console.WriteLine("Enter condition (optional):");
            var condition = Console.ReadLine();

            _bookCopyService.UpdateBookCopy(bookCopyId, bookId, barcode, shelfLocation, status, damagePercentage, condition);
            Console.WriteLine("Book copy updated.");
        }

        private void DeleteBookCopy()
        {
            Console.WriteLine("Enter book copy id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookCopyId)) return;
            _bookCopyService.RemoveBookCopy(bookCopyId);
            Console.WriteLine("Book copy deleted.");
        }

        private void ViewByBookId()
        {
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            foreach (var copy in _bookCopyService.GetCopiesByBookId(bookId))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void ViewAvailableByBookId()
        {
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            foreach (var copy in _bookCopyService.GetAvailableCopiesByBookId(bookId))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void SearchByStatus()
        {
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            Console.WriteLine("Enter status:");
            if (!Enum.TryParse(Console.ReadLine(), true, out BookStatus status)) return;
            foreach (var copy in _bookCopyService.GetCopiesByStatus(bookId, status))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void SearchByCondition()
        {
            Console.WriteLine("Enter book id:");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId)) return;
            Console.WriteLine("Enter condition term:");
            var condition = Console.ReadLine() ?? string.Empty;
            foreach (var copy in _bookCopyService.GetCopiesByCondition(bookId, condition))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Condition: {copy.Condition}");
            }
        }
    }
}