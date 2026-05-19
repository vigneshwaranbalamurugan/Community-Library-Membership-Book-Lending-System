using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Security;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;
using ModelMember = BookLendingApp.ModelLibrary.Models.Member;

namespace BookLendingApp.Application.Borrow
{
    public class BorrowApp
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly AppSession _session;

        public BorrowApp(IBorrowingService borrowingService, IBookCopyService bookCopyService, IBookService bookService, IBookCategoryService bookCategoryService, AppSession session)
        {
            _borrowingService = borrowingService;
            _bookCopyService = bookCopyService;
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _session = session;
        }

        public void BorrowMenu()
        {
            while (true)
            {
                ConsoleUi.WriteTitle("Borrow Menu");
                ConsoleUi.WriteMenuOptions(new[] { "Borrow Book", "Borrow by Category", "My Borrowing Summary", "Back" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 4);

                switch (choice)
                {
                    case 1: BorrowBook(); break;
                    case 2: BorrowBookByCategory(); break;
                    case 3: MemberSummary(); break;
                    case 4: return;
                    default: ConsoleUi.WriteError("Invalid choice."); ConsoleUi.Pause(); break;
                }
            }
        }

        private void BorrowBook()
        {
            if (!TryGetCurrentMember(out var member)) return;

            // 1. Select Book first
            var books = _bookService.GetAllBooks() ?? new List<BookLendingApp.ModelLibrary.Models.Book>();
            if (books.Count == 0)
            {
                ConsoleUi.WriteInfo("No books available in the library.");
                ConsoleUi.Pause();
                return;
            }

            var categories = _bookCategoryService.GetAllCategories() ?? new List<BookCategory>();
            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            var selectedBook = ConsoleInputValidator.PromptSelection(
                "Select a book to borrow:",
                books,
                book =>
                {
                    var catName = categoryMap.TryGetValue(book.CategoryId, out var name) ? name : "N/A";
                    return $"Title: {book.Title} | Author: {book.Author} | Category: {catName} | ISBN: {book.ISBN}";
                });

            // 2. Select available copy for that book
            var availableCopies = _bookCopyService.GetAvailableCopiesByBookId(selectedBook.BookId) ?? new List<BookCopy>();
            if (availableCopies.Count == 0)
            {
                ConsoleUi.WriteError($"No available copies for '{selectedBook.Title}'.");
                ConsoleUi.Pause();
                return;
            }

            var selectedCopy = ConsoleInputValidator.PromptSelection(
                "Select a copy:",
                availableCopies,
                copy => $"Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status} | Damage: {copy.DamagePercentage}%");

            try
            {
                if (!_borrowingService.CanBorrow(member.MemberId, selectedBook.BookId, out var validationMessage))
                {
                    ConsoleUi.WriteError($"Cannot borrow: {validationMessage}");
                    ConsoleUi.Pause();
                    return;
                }

                var borrow = _borrowingService.BorrowBook(member.MemberId, selectedCopy.BookCopyId);
                if (borrow == null)
                {
                    ConsoleUi.WriteError("Borrowing failed. You may have pending fines or borrowing limits.");
                    ConsoleUi.Pause();
                    return;
                }

                ConsoleUi.WriteSuccess($"Success! Book borrowed. Due date: {borrow.BorrowDate.AddDays(7):d}");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Borrow failed: {ex.Message}");
                ConsoleUi.Pause();
            }
        }


        private void MemberSummary()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var summary = _borrowingService.GetMemberBorrowingSummary(member.MemberId);
            
            ConsoleUi.WriteTitle("My Borrowing Summary");
            
            var rows = new System.Collections.Generic.List<string>
            {
                $"Metric: Active Borrowings | Count: {summary.ActiveBorrows}",
                $"Metric: Returned Books | Count: {summary.ReturnedBorrows}",
                $"Metric: Total Unpaid Fine | Count: ₹{summary.TotalUnpaidFine:N2}"
            };

            ConsoleUi.WriteTable(rows);
            
            if (summary.TotalUnpaidFine > 0)
            {
                ConsoleUi.WriteError("\nYou have pending fines. Please settle them at the Payment counter.");
            }
            
            ConsoleUi.Pause();
        }

        private void BorrowBookByCategory()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var categories = _bookCategoryService.GetAllCategories() ?? new List<BookCategory>();
                if (categories.Count == 0)
                {
                    ConsoleUi.WriteInfo("No categories found.");
                    ConsoleUi.Pause();
                    return;
                }

            var selectedCategory = ConsoleInputValidator.PromptSelection(
                "Select a category:",
                categories,
                category => category.Name);

            var availableBooks = _borrowingService.GetAvailableBooksByCategory(selectedCategory.CategoryId);
                if (availableBooks.Count == 0)
                {
                    ConsoleUi.WriteInfo("No available books found for this category.");
                    ConsoleUi.Pause();
                    return;
                }

            var selectedBook = ConsoleInputValidator.PromptSelection(
                "Select a book:",
                availableBooks,
                book => $"Title: {book.Title} | Author: {book.Author} | ISBN: {book.Isbn} | Available copies: {book.AvailableCopies}");

            var availableCopies = _bookCopyService.GetAvailableCopiesByBookId(selectedBook.BookId) ?? new List<BookCopy>();
                if (availableCopies.Count == 0)
                {
                    ConsoleUi.WriteInfo("No available copies found for this book.");
                    ConsoleUi.Pause();
                    return;
                }

            var selectedCopy = ConsoleInputValidator.PromptSelection(
                "Select a copy:",
                availableCopies,
                copy => $"Barcode: {copy.Barcode} | Shelf: {copy.ShelfLocation} | Status: {copy.Status}");

            try
            {
                var copy = _bookCopyService.GetBookCopyById(selectedCopy.BookCopyId);
                if (copy == null)
                {
                    ConsoleUi.WriteError("Selected book copy was not found.");
                    ConsoleUi.Pause();
                    return;
                }

                if (!_borrowingService.CanBorrow(member.MemberId, copy.BookId, out var validationMessage))
                {
                    ConsoleUi.WriteError($"Cannot borrow: {validationMessage}");
                    ConsoleUi.Pause();
                    return;
                }

                var borrow = _borrowingService.BorrowBook(member.MemberId, selectedCopy.BookCopyId);
                if (borrow == null)
                {
                    ConsoleUi.WriteError("Borrowing failed. You may have pending fines or borrowing limits.");
                    ConsoleUi.Pause();
                    return;
                }

                ConsoleUi.WriteSuccess($"Borrowed. BorrowRecordId: {borrow.BorrowRecordId}");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Borrow failed: {ex.Message}");
            }
        }

        private Guid PromptAvailableBookCopySelection()
        {
            var copies = (_bookCopyService.GetAllBookCopies() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCopy>())
                .Where(copy => copy.Status == BookStatus.Available || copy.Status == BookStatus.Damaged)
                .ToList();

            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);
            var categories = _bookCategoryService.GetAllCategories() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.BookCategory>();
            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            if (copies.Count == 0)
            {
                ConsoleUi.WriteInfo("No available book copies found.");
                ConsoleUi.Pause();
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a book copy:",
                copies,
                copy =>
                {
                    var book = bookMap.TryGetValue(copy.BookId, out var b) ? b : copy.Book;
                    var title = book?.Title ?? copy.BookId.ToString();
                    var author = book?.Author ?? "";
                    var categoryName = (book != null && book.CategoryId != Guid.Empty && categoryMap.TryGetValue(book.CategoryId, out var cn)) ? cn : string.Empty;
                    var statusDisplay = copy.Status == BookStatus.Available ? "[Available]" : $"[Damaged: {copy.DamagePercentage}%]";
                    return $"Title: {title} | Author: {author} | Category: {categoryName} | Barcode: {copy.Barcode} | {statusDisplay}";
                });

            return selected.BookCopyId;
        }


        private bool TryGetCurrentMember(out ModelMember member)
        {
            member = _session.CurrentMember!;
            if (_session.IsMember && member != null)
            {
                return true;
            }

            ConsoleUi.WriteInfo("Please sign in as a user to access borrowing features.");
            ConsoleUi.Pause();
            return false;
        }
    }
}