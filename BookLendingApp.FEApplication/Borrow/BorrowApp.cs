using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Security;
using BookLendingApp.FEApplication.Validation;
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
                Console.WriteLine("Borrow Menu:");
                Console.WriteLine("1. Borrow Book");
                Console.WriteLine("2. My Borrowing Summary");
                Console.WriteLine("3. Back");

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 3);

                switch (choice)
                {
                    case 1: BorrowBook(); break;
                    case 2: MemberSummary(); break;
                    case 3: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void BorrowBook()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var bookCopyId = PromptAvailableBookCopySelection();
            if (bookCopyId == Guid.Empty) return;

            try
            {
                var borrow = _borrowingService.BorrowBook(member.MemberId, bookCopyId);
                Console.WriteLine($"Borrowed. BorrowRecordId: {borrow.BorrowRecordId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Borrow failed: {ex.Message}");
            }
        }


        private void MemberSummary()
        {
            if (!TryGetCurrentMember(out var member)) return;

            var summary = _borrowingService.GetMemberBorrowingSummary(member.MemberId);
            Console.WriteLine($"Active borrows: {summary.ActiveBorrows}");
            Console.WriteLine($"Returned borrows: {summary.ReturnedBorrows}");
            Console.WriteLine($"Unpaid fine: ₹{summary.TotalUnpaidFine}");
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
                Console.WriteLine("No available book copies found.");
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

            Console.WriteLine("Please sign in as a user to access borrowing features.");
            return false;
        }
    }
}