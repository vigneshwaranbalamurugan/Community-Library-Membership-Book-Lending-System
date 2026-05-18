using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.FEApplication.Validation;

namespace BookLendingApp.Application.Borrow
{
    public class BookCopyApp
    {
        private readonly IBookService _bookService;
        private readonly IBookCopyService _bookCopyService;

        public BookCopyApp(IBookService bookService, IBookCopyService bookCopyService)
        {
            _bookService = bookService;
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

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 9);

                switch (choice)
                {
                    case 1: AddBookCopy(); break;
                    case 2: ViewBookCopies(); break;
                    case 3: UpdateBookCopy(); break;
                    case 4: DeleteBookCopy(); break;
                    case 5: ViewByBookId(); break;
                    case 6: ViewAvailableByBookId(); break;
                    case 7: SearchByStatus(); break;
                    case 8: SearchByCondition(); break;
                    case 9: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void AddBookCopy()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;

            var barcode = ConsoleInputValidator.ReadRequiredString("Enter barcode:");
            var shelfLocation = ConsoleInputValidator.ReadRequiredString("Enter shelf location:");
            var status = ConsoleInputValidator.PromptEnumSelection<BookStatus>("Select a status:");
            var damagePercentage = ConsoleInputValidator.ReadDecimal("Enter damage percentage:", 0m, 100m);
            var condition = ConsoleInputValidator.ReadOptionalString("Enter condition (optional):");

            _bookCopyService.AddBookCopy(bookId, barcode, shelfLocation, status, damagePercentage, condition);
            Console.WriteLine("Book copy added.");
        }

        private void ViewBookCopies()
        {
            var copies = _bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                Console.WriteLine("No book copies found.");
                return;
            }

            foreach (var copy in copies)
            {
                var title = copy.Book?.Title ?? copy.BookId.ToString();
                Console.WriteLine($"Book: {title} | Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status} | Condition: {copy.Condition}");
            }
        }

        private void UpdateBookCopy()
        {
            var bookCopyId = PromptBookCopySelection();
            if (bookCopyId == Guid.Empty) return;

            var existingCopy = _bookCopyService.GetBookCopyById(bookCopyId);
            if (existingCopy == null)
            {
                Console.WriteLine("Book copy not found.");
                return;
            }

            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            var titleMap = books.ToDictionary(book => book.BookId, book => book.Title);
            var currentBookTitle = titleMap.TryGetValue(existingCopy.BookId, out var currentTitle) ? currentTitle : existingCopy.BookId.ToString();
            Console.WriteLine($"Current values: Book={currentBookTitle}, Barcode={existingCopy.Barcode}, Shelf={existingCopy.ShelfLocation}, Status={existingCopy.Status}, Damage={existingCopy.DamagePercentage}, Condition={existingCopy.Condition}");

            var bookId = existingCopy.BookId;
            if (ConsoleInputValidator.ReadYesNo($"Change book? Current is '{currentBookTitle}'", defaultValue: false))
            {
                bookId = PromptBookSelection();
                if (bookId == Guid.Empty) return;
            }

            var barcode = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter barcode", existingCopy.Barcode);
            var shelfLocation = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter shelf location", existingCopy.ShelfLocation);
            var status = ConsoleInputValidator.PromptEnumSelectionWithDefault("Select a status", existingCopy.Status);
            var damagePercentage = ConsoleInputValidator.ReadDecimalWithDefault("Enter damage percentage", existingCopy.DamagePercentage, 0m, 100m);
            var condition = ConsoleInputValidator.ReadOptionalStringWithDefault("Enter condition (optional)", existingCopy.Condition);

            _bookCopyService.UpdateBookCopy(bookCopyId, bookId, barcode, shelfLocation, status, damagePercentage, condition);
            Console.WriteLine("Book copy updated.");
        }

        private void DeleteBookCopy()
        {
            var bookCopyId = PromptBookCopySelection();
            if (bookCopyId == Guid.Empty) return;

            _bookCopyService.RemoveBookCopy(bookCopyId);
            Console.WriteLine("Book copy deleted.");
        }

        private void ViewByBookId()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;
            var copies = _bookCopyService.GetCopiesByBookId(bookId) ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                Console.WriteLine("No copies found for the selected book.");
                return;
            }

            foreach (var copy in copies)
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void ViewAvailableByBookId()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;
            var copies = _bookCopyService.GetAvailableCopiesByBookId(bookId) ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                Console.WriteLine("No available copies found for the selected book.");
                return;
            }

            foreach (var copy in copies)
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void SearchByStatus()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;

            var status = ConsoleInputValidator.PromptEnumSelection<BookStatus>("Select a status:");
            foreach (var copy in _bookCopyService.GetCopiesByStatus(bookId, status))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
        }

        private void SearchByCondition()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;

            var condition = ConsoleInputValidator.ReadRequiredString("Enter condition term:");
            foreach (var copy in _bookCopyService.GetCopiesByCondition(bookId, condition))
            {
                Console.WriteLine($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Condition: {copy.Condition}");
            }
        }

        private Guid PromptBookSelection()
        {
            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            if (books.Count == 0)
            {
                Console.WriteLine("No books found. Add a book first.");
                return Guid.Empty;
            }

            var selectedBook = ConsoleInputValidator.PromptSelection(
                "Select a book:",
                books,
                book => $"{book.Title} | {book.Author} | ISBN: {book.ISBN}");

            return selectedBook.BookId;
        }

        private Guid PromptBookCopySelection()
        {
            var copies = _bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                Console.WriteLine("No book copies found.");
                return Guid.Empty;
            }

            var selectedCopy = ConsoleInputValidator.PromptSelection(
                "Select a book copy:",
                copies,
                copy =>
                {
                    var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
                    var titleMap = books.ToDictionary(b => b.BookId, b => b.Title);
                    var title = titleMap.TryGetValue(copy.BookId, out var t) ? t : copy.BookId.ToString();
                    return $"Book: {title} | Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status}";
                });

            return selectedCopy.BookCopyId;
        }
    }
}