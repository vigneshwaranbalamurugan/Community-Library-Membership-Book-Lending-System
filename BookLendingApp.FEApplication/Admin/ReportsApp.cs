using System;
using System.Collections.Generic;
using System.Linq;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;
using ModelMember = BookLendingApp.ModelLibrary.Models.Member;
using ModelBook = BookLendingApp.ModelLibrary.Models.Book;

namespace BookLendingApp.Application.Admin
{
    public class ReportsApp
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IMemberService _memberService;
        private readonly IBookService _bookService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookCategoryService _bookCategoryService;

        public ReportsApp(IBorrowingService borrowingService, IMemberService memberService, IBookService bookService, IBookCopyService bookCopyService, IBookCategoryService bookCategoryService)
        {
            _borrowingService = borrowingService;
            _memberService = memberService;
            _bookService = bookService;
            _bookCopyService = bookCopyService;
            _bookCategoryService = bookCategoryService;
        }

        public void ReportsMenu()
        {
            while (true)
            {
                ConsoleUi.WriteTitle("Reports Menu");
                ConsoleUi.WriteMenuOptions(new[] { "Books currently borrowed", "Overdue books", "Members with pending fines", "Most borrowed books", "Available books by category", "Member borrowing history", "Back" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 7);
                switch (choice)
                {
                    case 1: ShowBooksCurrentlyBorrowed(); break;
                    case 2: ShowOverdueBooks(); break;
                    case 3: ShowMembersWithPendingFines(); break;
                    case 4: ShowMostBorrowedBooks(); break;
                    case 5: ShowAvailableBooksByCategory(); break;
                    case 6: ShowMemberBorrowingHistory(); break;
                    case 7: return;
                }
            }
        }

        private void ShowBooksCurrentlyBorrowed()
        {
            var records = _borrowingService.GetAllActiveBorrowRecords();
            if (records == null || records.Count == 0)
            {
                ConsoleUi.WriteInfo("No books are currently borrowed.");
                ConsoleUi.Pause();
                return;
            }

            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            var memberMap = members.ToDictionary(m => m.MemberId, m => m);

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            var rows = new System.Collections.Generic.List<string>();
            foreach (var r in records)
            {
                var member = memberMap.TryGetValue(r.MemberId, out var m) ? m : null;
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                var dueDate = r.BorrowDate.AddDays(r.RenewalDays);
                rows.Add($"Book: {book?.Title ?? "Unknown"} | Member: {member?.FullName ?? "Unknown"} | Member Mobile: {member?.MobileNumber ?? "N/A"} | Due: {dueDate:d} | Copy ID: {r.BookCopyId}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ShowOverdueBooks()
        {
            var records = _borrowingService.GetOverdueBorrowRecords();
            if (records == null || records.Count == 0)
            {
                ConsoleUi.WriteInfo("No overdue books.");
                ConsoleUi.Pause();
                return;
            }

            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            var memberMap = members.ToDictionary(m => m.MemberId, m => m);

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            var rows = new System.Collections.Generic.List<string>();
            foreach (var r in records)
            {
                var member = memberMap.TryGetValue(r.MemberId, out var m) ? m : null;
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                var dueDate = r.BorrowDate.AddDays(r.RenewalDays);
                var daysOverdue = (int)(DateTime.UtcNow.Date - dueDate.Date).TotalDays;
                rows.Add($"Member: {member?.FullName ?? "Unknown"} | Email: {member?.EmailId ?? "N/A"} | Mobile: {member?.MobileNumber ?? "N/A"} | Book: {book?.Title ?? "Unknown"} | Copy: {r.BookCopyId} | Due: {dueDate:d} | Days overdue: {daysOverdue}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ShowMembersWithPendingFines()
        {
            var list = _borrowingService.GetMembersWithPendingFines();
            if (list == null || list.Count == 0)
            {
                ConsoleUi.WriteInfo("No members with pending fines.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var m in list)
            {
                rows.Add($"Name: {m.FullName} | Email: {m.EmailId} | Pending: ₹{m.UnpaidAmount}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ShowMostBorrowedBooks()
        {
            var top = ConsoleInputValidator.ReadInt("How many top books to show?", 1, 100);
            var list = _borrowingService.GetMostBorrowedBooks(top);
            if (list == null || list.Count == 0)
            {
                ConsoleUi.WriteInfo("No borrowing data available.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var b in list)
            {
                rows.Add($"Title: {b.Title} | Author: {b.Author} | ISBN: {b.Isbn} | Borrowed: {b.BorrowCount}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ShowAvailableBooksByCategory()
        {
            var categories = _bookCategoryService.GetAllCategories();
            if (categories == null || categories.Count == 0)
            {
                ConsoleUi.WriteInfo("No categories found.");
                ConsoleUi.Pause();
                return;
            }
            var category = ConsoleInputValidator.PromptSelection("Select a category:", categories, c => c.Name);
            var available = _borrowingService.GetAvailableBooksByCategory(category.CategoryId);
            if (available == null || available.Count == 0)
            {
                ConsoleUi.WriteInfo("No available books in this category.");
                ConsoleUi.Pause();
                return;
            }

            var rows = new System.Collections.Generic.List<string>();
            foreach (var a in available)
            {
                rows.Add($"Title: {a.Title} | Author: {a.Author} | ISBN: {a.Isbn} | Available Copies: {a.AvailableCopies}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

        private void ShowMemberBorrowingHistory()
        {
            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            if (members.Count == 0)
            {
                ConsoleUi.WriteInfo("No members found.");
                ConsoleUi.Pause();
                return;
            }

            var member = ConsoleInputValidator.PromptSelection<ModelMember>("Select a member:", members, m => $"{m.FullName} | {m.EmailId}");
            var history = _borrowingService.GetBorrowHistoryByMember(member.MemberId);
            if (history == null || history.Count == 0)
            {
                ConsoleUi.WriteInfo("No borrowing history for this member.");
                ConsoleUi.Pause();
                return;
            }

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            var rows = new System.Collections.Generic.List<string>();
            foreach (var r in history)
            {
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                rows.Add($"ID: {r.BorrowRecordId} | Book: {book?.Title ?? "Unknown"} | Borrowed: {r.BorrowDate:d} | Returned: {(r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("d") : "Not returned")}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }
    }
}