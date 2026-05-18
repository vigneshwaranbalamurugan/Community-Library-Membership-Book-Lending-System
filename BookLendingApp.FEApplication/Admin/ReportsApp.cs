using System;
using System.Collections.Generic;
using System.Linq;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Validation;
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
                Console.WriteLine("Reports Menu:");
                Console.WriteLine("1. Books currently borrowed");
                Console.WriteLine("2. Overdue books");
                Console.WriteLine("3. Members with pending fines");
                Console.WriteLine("4. Most borrowed books");
                Console.WriteLine("5. Available books by category");
                Console.WriteLine("6. Member borrowing history");
                Console.WriteLine("7. Back");

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
                Console.WriteLine("No books are currently borrowed.");
                return;
            }

            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            var memberMap = members.ToDictionary(m => m.MemberId, m => m);

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            Console.WriteLine("Books currently borrowed:");
            foreach (var r in records)
            {
                var member = memberMap.TryGetValue(r.MemberId, out var m) ? m : null;
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                var dueDate = r.BorrowDate.AddDays(r.RenewalDays);
                Console.WriteLine($"Book: {book?.Title ?? "Unknown"} | Member: {member?.FullName ?? "Unknown"} |Member Mobile: {member?.MobileNumber ?? "N/A"}| Due: {dueDate:d} | Copy ID: {r.BookCopyId}");
            }
        }

        private void ShowOverdueBooks()
        {
            var records = _borrowingService.GetOverdueBorrowRecords();
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("No overdue books.");
                return;
            }

            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            var memberMap = members.ToDictionary(m => m.MemberId, m => m);

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            Console.WriteLine("Overdue Books:");
            foreach (var r in records)
            {
                var member = memberMap.TryGetValue(r.MemberId, out var m) ? m : null;
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                var dueDate = r.BorrowDate.AddDays(r.RenewalDays);
                var daysOverdue = (int)(DateTime.UtcNow.Date - dueDate.Date).TotalDays;
                Console.WriteLine($"Member: {member?.FullName ?? "Unknown"} | Email: {member?.EmailId ?? "N/A"} | Mobile: {member?.MobileNumber ?? "N/A"}");
                Console.WriteLine($"  Book: {book?.Title ?? "Unknown"} | Copy: {r.BookCopyId}");
                Console.WriteLine($"  Due: {dueDate:d} | Days overdue: {daysOverdue}");
                Console.WriteLine();
            }
        }

        private void ShowMembersWithPendingFines()
        {
            var list = _borrowingService.GetMembersWithPendingFines();
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("No members with pending fines.");
                return;
            }

            Console.WriteLine("Members with pending fines:");
            foreach (var m in list)
            {
                Console.WriteLine($"{m.FullName} | {m.EmailId} | Pending: ₹{m.UnpaidAmount}");
            }
        }

        private void ShowMostBorrowedBooks()
        {
            var top = ConsoleInputValidator.ReadInt("How many top books to show?", 1, 100);
            var list = _borrowingService.GetMostBorrowedBooks(top);
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("No borrowing data available.");
                return;
            }

            Console.WriteLine($"Top {list.Count} most borrowed books:");
            foreach (var b in list)
            {
                Console.WriteLine($"{b.Title} | {b.Author} | ISBN: {b.Isbn} | Borrowed: {b.BorrowCount}");
            }
        }

        private void ShowAvailableBooksByCategory()
        {
            var categories = _bookCategoryService.GetAllCategories();
            if (categories == null || categories.Count == 0)
            {
                Console.WriteLine("No categories found.");
                return;
            }
            var category = ConsoleInputValidator.PromptSelection("Select a category:", categories, c => c.Name);
            var available = _borrowingService.GetAvailableBooksByCategory(category.CategoryId);
            if (available == null || available.Count == 0)
            {
                Console.WriteLine("No available books in this category.");
                return;
            }

            Console.WriteLine($"Available books in {category.Name}:");
            foreach (var a in available)
            {
                Console.WriteLine($"{a.Title} | {a.Author} | ISBN: {a.Isbn} | Available Copies: {a.AvailableCopies}");
            }
        }

        private void ShowMemberBorrowingHistory()
        {
            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found.");
                return;
            }

            var member = ConsoleInputValidator.PromptSelection<ModelMember>("Select a member:", members, m => $"{m.FullName} | {m.EmailId}");
            var history = _borrowingService.GetBorrowHistoryByMember(member.MemberId);
            if (history == null || history.Count == 0)
            {
                Console.WriteLine("No borrowing history for this member.");
                return;
            }

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            Console.WriteLine($"Borrowing history for {member.FullName}:");
            foreach (var r in history)
            {
                var copy = _bookCopyService.GetBookCopyById(r.BookCopyId);
                var book = copy != null && bookMap.TryGetValue(copy.BookId, out var b) ? b : null;
                Console.WriteLine($"ID: {r.BorrowRecordId} | Book: {book?.Title ?? "Unknown"} | Borrowed: {r.BorrowDate:d} | Returned: {(r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("d") : "Not returned")}");
            }
        }
    }
    }