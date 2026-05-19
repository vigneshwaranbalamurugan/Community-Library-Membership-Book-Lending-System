using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

namespace BookLendingApp.Application.Borrow
{
    public class BookCopyApp
    {
        private readonly IBookService _bookService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookCategoryService _bookCategoryService;

        public BookCopyApp(IBookService bookService, IBookCopyService bookCopyService, IBookCategoryService bookCategoryService)
        {
            _bookService = bookService;
            _bookCopyService = bookCopyService;
            _bookCategoryService = bookCategoryService;
        }

        public void BookCopyMenu()
        {
            while (true)
            {
                ConsoleUi.WriteTitle("Book Copy Menu");
                ConsoleUi.WriteMenuOptions(new[] { "Add Book Copy", "View All Book Copies", "View Book Copies By Category", "Update Book Copy", "Delete Book Copy", "View By Book Id", "View Available By Book Id", "Search By Status", "Search By Condition", "Back" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 10);

                switch (choice)
                {
                    case 1: AddBookCopy(); break;
                    case 2: ViewBookCopies(); break;
                    case 3: ViewCopiesByCategory(); break;
                    case 4: UpdateBookCopy(); break;
                    case 5: DeleteBookCopy(); break;
                    case 6: ViewByBookId(); break;
                    case 7: ViewAvailableByBookId(); break;
                    case 8: SearchByStatus(); break;
                    case 9: SearchByCondition(); break;
                    case 10: return;
                    default: ConsoleUi.WriteError("Invalid choice."); break;
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
            ConsoleUi.WriteSuccess("Book copy added.");
            ConsoleUi.Pause();
        }

        private void ViewBookCopies()
        {
            var copies = _bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                ConsoleUi.WriteInfo("No book copies found.");
                ConsoleUi.Pause();
                return;
            }
            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in copies)
            {
                var title = copy.Book?.Title ?? copy.BookId.ToString();
                rows.Add($"Book: {title} | Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status} | Condition: {copy.Condition}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void UpdateBookCopy()
        {
            var bookCopyId = PromptBookCopySelection();
            if (bookCopyId == Guid.Empty) return;

            var existingCopy = _bookCopyService.GetBookCopyById(bookCopyId);
            if (existingCopy == null)
            {
                ConsoleUi.WriteError("Book copy not found.");
                return;
            }

            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            var titleMap = books.ToDictionary(book => book.BookId, book => book.Title);
            var currentBookTitle = titleMap.TryGetValue(existingCopy.BookId, out var currentTitle) ? currentTitle : existingCopy.BookId.ToString();
            ConsoleUi.WriteInfo($"Current values: Book={currentBookTitle}, Barcode={existingCopy.Barcode}, Shelf={existingCopy.ShelfLocation}, Status={existingCopy.Status}, Damage={existingCopy.DamagePercentage}, Condition={existingCopy.Condition}");

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
            ConsoleUi.WriteSuccess("Book copy updated.");
            ConsoleUi.Pause();
        }

        private void DeleteBookCopy()
        {
            var bookCopyId = PromptBookCopySelection();
            if (bookCopyId == Guid.Empty) return;

            try
            {
                _bookCopyService.RemoveBookCopy(bookCopyId);
                ConsoleUi.WriteSuccess("Book copy deleted.");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Cannot delete book copy: {ex.Message}");
                ConsoleUi.WriteInfo("Note: Ensure this copy is not currently borrowed or linked to history.");
            }
            ConsoleUi.Pause();
        }

        private void ViewByBookId()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;
            var copies = _bookCopyService.GetCopiesByBookId(bookId) ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                ConsoleUi.WriteInfo("No copies found for the selected book.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in copies)
            {
                rows.Add($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ViewAvailableByBookId()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;
            var copies = _bookCopyService.GetAvailableCopiesByBookId(bookId) ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                ConsoleUi.WriteInfo("No available copies found for the selected book.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in copies)
            {
                rows.Add($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void SearchByStatus()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;

            var status = ConsoleInputValidator.PromptEnumSelection<BookStatus>("Select a status:");
            var list = _bookCopyService.GetCopiesByStatus(bookId, status);
            
            if (list == null || list.Count == 0)
            {
                ConsoleUi.WriteInfo($"No copies found with status '{status}' for this book.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in list)
            {
                rows.Add($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Status: {copy.Status}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void SearchByCondition()
        {
            var bookId = PromptBookSelection();
            if (bookId == Guid.Empty) return;

            var condition = ConsoleInputValidator.ReadRequiredString("Enter condition term:");
            var list = _bookCopyService.GetCopiesByCondition(bookId, condition);

            if (list == null || list.Count == 0)
            {
                ConsoleUi.WriteInfo($"No copies found matching condition '{condition}' for this book.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in list)
            {
                rows.Add($"ID: {copy.BookCopyId} | Barcode: {copy.Barcode} | Condition: {copy.Condition}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private Guid PromptBookSelection()
        {
            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            if (books.Count == 0)
            {
                ConsoleUi.WriteInfo("No books found. Add a book first.");
                return Guid.Empty;
            }

            var selectedBook = ConsoleInputValidator.PromptSelection(
                "Select a book:",
                books,
                book => $"Title: {book.Title} | Author: {book.Author} | ISBN: {book.ISBN}");

            return selectedBook.BookId;
        }

        private Guid PromptBookCopySelection()
        {
            var copies = _bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            if (copies.Count == 0)
            {
                ConsoleUi.WriteInfo("No book copies found.");
                ConsoleUi.Pause();
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

        public void ViewCopiesByCategory()
        {
            var categories = _bookCategoryService.GetAllCategories() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCategory>();
            if (categories.Count == 0)
            {
                ConsoleUi.WriteInfo("No categories found.");
                ConsoleUi.Pause();
                return;
            }

            var selectedCategory = ConsoleInputValidator.PromptSelection(
                "Select a category to view copies:",
                categories,
                cat => cat.Name);

            var booksInCategory = (_bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>())
                                  .Where(b => b.CategoryId == selectedCategory.CategoryId)
                                  .ToList();

            if (booksInCategory.Count == 0)
            {
                ConsoleUi.WriteInfo($"No books found in category '{selectedCategory.Name}'.");
                ConsoleUi.Pause();
                return;
            }

            var bookIds = booksInCategory.Select(b => b.BookId).ToHashSet();
            var allCopies = _bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>();
            
            var filteredCopies = allCopies.Where(c => bookIds.Contains(c.BookId)).ToList();

            if (filteredCopies.Count == 0)
            {
                ConsoleUi.WriteInfo($"No book copies found for category '{selectedCategory.Name}'.");
                ConsoleUi.Pause();
                return;
            }

            var bookTitleMap = booksInCategory.ToDictionary(b => b.BookId, b => b.Title);

            ConsoleUi.WriteTitle($"Copies in Category: {selectedCategory.Name}");
            var rows = new System.Collections.Generic.List<string>();
            foreach (var copy in filteredCopies)
            {
                var title = bookTitleMap.TryGetValue(copy.BookId, out var t) ? t : "Unknown Book";
                rows.Add($"Book: {title} | Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status}");
            }
            
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }
    }
}