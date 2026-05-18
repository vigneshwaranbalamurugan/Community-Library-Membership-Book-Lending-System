using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.DALLibrary.Interfaces;
using ModelMember = BookLendingApp.ModelLibrary.Models.Member;
using ModelBook = BookLendingApp.ModelLibrary.Models.Book;
using ModelPayment = BookLendingApp.ModelLibrary.Models.Payment;

namespace BookLendingApp.Application.Admin
{
    public class AdminBorrowApp
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IMemberService _memberService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookService _bookService;
        private readonly IFineManagementService _fineManagementService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IFineRuleService _fineRuleService;

        public AdminBorrowApp(
            IBorrowingService borrowingService,
            IMemberService memberService,
            IBookCopyService bookCopyService,
            IBookService bookService,
            IFineManagementService fineManagementService,
            IPaymentRepository paymentRepository,
            IFineRuleService fineRuleService)
        {
            _borrowingService = borrowingService;
            _memberService = memberService;
            _bookCopyService = bookCopyService;
            _bookService = bookService;
            _fineManagementService = fineManagementService;
            _paymentRepository = paymentRepository;
            _fineRuleService = fineRuleService;
        }

        public void BorrowManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("Borrow Management Menu:");
                Console.WriteLine("1. View Active Borrows");
                Console.WriteLine("2. Process Return");
                Console.WriteLine("3. View Overdue Books");
                Console.WriteLine("4. Back");

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 4);

                switch (choice)
                {
                    case 1: ViewActiveBorrows(); break;
                    case 2: ProcessReturn(); break;
                    case 3: ViewOverdueBooks(); break;
                    case 4: return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private void ViewActiveBorrows()
        {
            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found.");
                return;
            }

            var selectedMember = ConsoleInputValidator.PromptSelection<ModelMember>(
                "Select a member:",
                members,
                m => $"{m.FullName} | {m.EmailId}");

            var activeBorrows = _borrowingService.GetActiveBorrowRecords(selectedMember.MemberId) ?? new List<BorrowRecord>();
            if (activeBorrows.Count == 0)
            {
                Console.WriteLine($"No active borrows for {selectedMember.FullName}.");
                return;
            }

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            Console.WriteLine($"\nActive Borrows for {selectedMember.FullName}:");
            foreach (var borrow in activeBorrows)
            {
                var bookCopy = _bookCopyService.GetBookCopyById(borrow.BookCopyId);
                var book = bookMap.TryGetValue(bookCopy?.BookId ?? Guid.Empty, out var b) ? b : null;
                var title = book?.Title ?? "Unknown";
                var author = book?.Author ?? "Unknown";
                var dueDate = borrow.BorrowDate.AddDays(borrow.RenewalDays);
                var isOverdue = DateTime.UtcNow.Date > dueDate.Date;
                var condition = bookCopy?.DamagePercentage > 0 ? $" | Damage: {bookCopy.DamagePercentage}%" : "";

                Console.WriteLine($"  ID: {borrow.BorrowRecordId} | Book: {title} ({author}) | Borrowed: {borrow.BorrowDate:d} | Due: {dueDate:d}{condition}{(isOverdue ? " [OVERDUE]" : "")}");
            }
        }

        private void ProcessReturn()
        {
            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found.");
                return;
            }

            var selectedMember = ConsoleInputValidator.PromptSelection<ModelMember>(
                "Select a member:",
                members,
                m => $"{m.FullName} | {m.EmailId}");

            var activeBorrows = _borrowingService.GetActiveBorrowRecords(selectedMember.MemberId) ?? new List<BorrowRecord>();
            if (activeBorrows.Count == 0)
            {
                Console.WriteLine($"No active borrows for {selectedMember.FullName}.");
                return;
            }

            var selectedBorrow = ConsoleInputValidator.PromptSelection<BorrowRecord>(
                "Select a borrow to process return:",
                activeBorrows,
                borrow => $"ID: {borrow.BorrowRecordId} | Borrowed: {borrow.BorrowDate:d}");

            var returnDate = ConsoleInputValidator.ReadDateTime("Enter return date (yyyy-MM-dd):");

            Console.WriteLine("\nReturn Status:");
            Console.WriteLine("1. Returned in Good Condition");
            Console.WriteLine("2. Returned Damaged");
            Console.WriteLine("3. Lost");

            var statusChoice = ConsoleInputValidator.ReadInt("Select status:", 1, 3);

            decimal damagePercentage = 0;
            string notes = "";
            decimal additionalFine = 0;

            switch (statusChoice)
            {
                case 1:
                    notes = "Returned in good condition";
                    break;
                case 2:
                    damagePercentage = ConsoleInputValidator.ReadDecimal("Enter damage percentage (0-100):", 0m, 100m);
                    notes = $"Returned with {damagePercentage}% damage";
                    
                    // Calculate damage fine based on FineRule
                    var damageRules = _fineRuleService.GetFineRulesByFineType(FineType.DamagedBook);
                    if (damageRules.Count > 0)
                    {
                        var damageRule = damageRules[0]; // Get first applicable rule
                        additionalFine = CalculateFine(damagePercentage, damageRule);
                    }
                    break;
                case 3:
                    damagePercentage = 100;
                    notes = "Book lost";
                    
                    // Calculate lost book fine based on FineRule
                    var lostRules = _fineRuleService.GetFineRulesByFineType(FineType.LostBook);
                    if (lostRules.Count > 0)
                    {
                        var lostRule = lostRules[0]; // Get first applicable rule
                        additionalFine = CalculateFine(damagePercentage, lostRule);
                    }
                    break;
            }

            try
            {
                // Process the return with damage percentage
                var returnedBorrow = _borrowingService.ReturnBook(selectedBorrow.BorrowRecordId, returnDate, damagePercentage);

                // If book is lost, mark it as Lost instead of deletion
                if (damagePercentage >= 100)
                {
                    var bookCopy = _bookCopyService.GetBookCopyById(selectedBorrow.BookCopyId);
                    if (bookCopy != null)
                    {
                        _bookCopyService.UpdateBookCopy(
                            bookCopy.BookCopyId,
                            bookCopy.BookId,
                            bookCopy.Barcode,
                            bookCopy.ShelfLocation,
                            BookStatus.Lost,
                            100,
                            "Lost during borrowing"
                        );
                    }
                }
                // If book is damaged, update its damage status
                else if (damagePercentage > 0)
                {
                    var bookCopy = _bookCopyService.GetBookCopyById(selectedBorrow.BookCopyId);
                    if (bookCopy != null)
                    {
                        _bookCopyService.UpdateBookCopy(
                            bookCopy.BookCopyId,
                            bookCopy.BookId,
                            bookCopy.Barcode,
                            bookCopy.ShelfLocation,
                            BookStatus.Damaged,
                            damagePercentage,
                            "Damaged during return"
                        );
                    }
                }

                Console.WriteLine("Return processed successfully.");

                // Apply additional fine for damage/loss if needed
                if (additionalFine > 0)
                {
                    ModelPayment payment;

                    if (damagePercentage < 100)
                    {
                        // Damage fee
                        payment = new BookDamageFeePayment
                        {
                            PaymentId = Guid.NewGuid(),
                            MemberId = selectedMember.MemberId,
                            BorrowRecordId = selectedBorrow.BorrowRecordId,
                            Amount = additionalFine,
                            PaymentDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        // Lost book fee
                        payment = new LostBookFeePayment
                        {
                            PaymentId = Guid.NewGuid(),
                            MemberId = selectedMember.MemberId,
                            BorrowRecordId = selectedBorrow.BorrowRecordId,
                            Amount = additionalFine,
                            PaymentDate = DateTime.UtcNow
                        };
                    }

                    _paymentRepository.Create(payment);
                    Console.WriteLine($"Additional fine applied: ₹{additionalFine} ({notes})");
                }

                Console.WriteLine($"Notes: {notes}");
                Console.WriteLine($"Total fine pending: ₹{_borrowingService.GetUnpaidFine(selectedMember.MemberId)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing return: {ex.Message}");
            }
        }

        private void ViewOverdueBooks()
        {
            var overdueRecords = _borrowingService.GetOverdueBorrowRecords();
            if (overdueRecords.Count == 0)
            {
                Console.WriteLine("No overdue books found.");
                return;
            }

            var members = _memberService.GetAllMembers() ?? new List<ModelMember>();
            var memberMap = members.ToDictionary(m => m.MemberId, m => m);

            var books = _bookService.GetAllBooks() ?? new List<ModelBook>();
            var bookMap = books.ToDictionary(b => b.BookId, b => b);

            Console.WriteLine("Overdue Books:");
            foreach (var record in overdueRecords)
            {
                var member = memberMap.TryGetValue(record.MemberId, out var m) ? m : null;
                var memberName = member?.FullName ?? "Unknown";
                var memberEmail = member?.EmailId ?? "N/A";

                var bookCopy = _bookCopyService.GetBookCopyById(record.BookCopyId);
                var book = bookMap.TryGetValue(bookCopy?.BookId ?? Guid.Empty, out var b) ? b : null;
                var title = book?.Title ?? "Unknown";

                var dueDate = record.BorrowDate.AddDays(record.RenewalDays);
                var daysOverdue = (int)(DateTime.UtcNow.Date - dueDate.Date).TotalDays;
                var lateFine = daysOverdue * 10m;

                Console.WriteLine($"  Member: {memberName} ({memberEmail})");
                Console.WriteLine($"  Book: {title} | Copy ID: {bookCopy?.BookCopyId}");
                Console.WriteLine($"  Due Date: {dueDate:d} | Days Overdue: {daysOverdue} | Late Fine: ₹{lateFine}");
                Console.WriteLine();
            }
        }

        private decimal CalculateFine(decimal damagePercentage, FineRule rule)
        {
            return rule.FineCalculationType switch
            {
                FineCalculationType.FlatFee => rule.Amount,
                FineCalculationType.Percentage =>
                    rule.Percentage is > 0
                        ? rule.Amount * (damagePercentage / rule.Percentage.Value)
                        : rule.Amount * damagePercentage,
                _ => rule.Amount
            };
        }
    }
}
