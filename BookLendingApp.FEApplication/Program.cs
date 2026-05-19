using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.Ballibrary.Services;
using BookLendingApp.Application.Book;
using BookLendingApp.Application.Member;
using BookLendingApp.Application.Borrow;
using BookLendingApp.Application.Payment;
using BookLendingApp.Application.Admin;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;
using BookLendingApp.FEApplication.Security;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.DALLibrary.Repositories;

namespace BookLendingApp.FEApplication
{
    internal class Program
    {
        private readonly BookLendingAppContext _context;
        private readonly IBookCategoryRepository _bookCategoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        private readonly IFineRuleRepository _fineRuleRepository;
        private readonly IBorrowRecordRepository _borrowRecordRepository;
        private readonly IPaymentRepository _paymentRepository;

        private readonly IBookCategoryService _bookCategoryService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IFineRuleService _fineRuleService;
        private readonly IBorrowingService _borrowingService;
        private readonly IFineManagementService _fineManagementService;
        private readonly AppAuthService _authService;
        private readonly AppSession _session;

        private readonly BookCategoryApp _bookCategoryApp;
        private readonly BookApp _bookApp;
        private readonly MemberApp _memberApp;
        private readonly MembershipApp _membershipApp;
        private readonly BookCopyApp _bookCopyApp;
        private readonly BorrowApp _borrowApp;
        private readonly FineManagementApp _fineManagementApp;
        private readonly FineRuleApp _fineRuleApp;
        private readonly ReportsApp _reportsApp;
        private readonly AdminBorrowApp _adminBorrowApp;

        Program()
        {
            _session = new AppSession();
            _context = new BookLendingAppContext();

            _bookRepository = new BookRepository(_context);
            _bookCategoryRepository = new BookCategoryRepository(_context);
            _memberRepository = new MemberRepository(_context);
            _membershipRepository = new MembershipRepository(_context);
            _bookCopyRepository = new BookCopyRepository(_context);
            _fineRuleRepository = new FineRuleRepository(_context);
            _borrowRecordRepository = new BorrowRecordRepository(_context);
            _paymentRepository = new PaymentRepository(_context);

            _bookCategoryService = new BookCategoryService(_bookCategoryRepository);
            _bookService = new BookService(_bookRepository, _bookCategoryRepository);
            _memberService = new MemberService(_memberRepository);
            _membershipService = new MembershipService(_membershipRepository);
            _bookCopyService = new BookCopyService(_bookCopyRepository);
            _fineRuleService = new FineRuleService(_fineRuleRepository);
            _borrowingService = new BorrowingService(_context, _memberRepository, _membershipRepository, _bookRepository, _bookCopyRepository, _borrowRecordRepository, _paymentRepository, _fineRuleRepository);
            _fineManagementService = new FineManagementService(_paymentRepository);
            _authService = new AppAuthService(_memberService);

            _bookCategoryApp = new BookCategoryApp(_bookCategoryService);
            _bookApp = new BookApp(_bookService, _bookCategoryService);
            _memberApp = new MemberApp(_memberService, _membershipService);
            _membershipApp = new MembershipApp(_membershipService);
            _bookCopyApp = new BookCopyApp(_bookService, _bookCopyService, _bookCategoryService);
            _borrowApp = new BorrowApp(_borrowingService, _bookCopyService, _bookService, _bookCategoryService, _session);
            _fineManagementApp = new FineManagementApp(_fineManagementService, _session);
            _fineRuleApp = new FineRuleApp(_fineRuleService);
            _reportsApp = new ReportsApp(_borrowingService, _memberService, _bookService, _bookCopyService, _bookCategoryService);
            _adminBorrowApp = new AdminBorrowApp(_borrowingService, _memberService, _bookCopyService, _bookService, _fineManagementService, _paymentRepository, _fineRuleService);
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        private void Run()
        {
            ConsoleUi.WriteTitle("WELCOME TO LIBRARY MANAGEMENT SYSTEM");
            ConsoleUi.WriteInfo($"Today's Date: {DateTime.Now:D}");
            ConsoleUi.WriteInfo("Please log in to continue.");
            ConsoleUi.Pause();

            while (true)
            {
                ConsoleUi.WriteTitle("Authentication");
                ConsoleUi.WriteMenuOptions(new[] { "Admin Login", "User Login", "Exit" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 3);
                switch (choice)
                {
                    case 1:
                        if (TryAdminLogin())
                        {
                            RunAdminMenu();
                        }
                        break;
                    case 2:
                        if (TryUserLogin())
                        {
                            RunUserMenu();
                        }
                        break;
                    case 3:
                        Console.Clear();
                        ConsoleUi.WriteTitle("THANK YOU FOR USING THE LIBRARY");
                        ConsoleUi.WriteSuccess("Exiting application... Have a great day!");
                        return;
                }
            }
        }

        private bool TryAdminLogin()
        {
            var username = ConsoleInputValidator.ReadRequiredString("Enter admin username:");
            var password = ConsoleInputValidator.ReadRequiredString("Enter admin password:");

            if (!_authService.ValidateAdmin(username, password))
            {
                ConsoleUi.WriteError("Invalid admin credentials.");
                ConsoleUi.Pause();
                return false;
            }

            _session.LoginAsAdmin();
            return true;
        }

        private bool TryUserLogin()
        {
            var email = ConsoleInputValidator.ReadEmail("Enter email:");
            var password = ConsoleInputValidator.ReadRequiredString("Enter password:");

            try
            {
                var member = _authService.AuthenticateMember(email, password);
                _session.LoginAsMember(member);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Login failed: {ex.Message}");
                ConsoleUi.Pause();
                return false;
            }
        }

        private void RunAdminMenu()
        {
            ConsoleUi.WriteSuccess("\nHello, Administrator! Login successful.");
            ConsoleUi.Pause();

            while (_session.IsAdmin)
            {
                ConsoleUi.WriteTitle("Admin Menu");
                ConsoleUi.WriteMenuOptions(new[] {
                    "Books",
                    "Categories",
                    "Members",
                    "Memberships",
                    "Book Copies",
                    "Borrow Management",
                    "Fine Rules",
                    "Reports",
                    "Logout"
                });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 9);
                switch (choice)
                {
                    case 1: _bookApp.BookMenu(); break;
                    case 2: _bookCategoryApp.CategoryMenu(); break;
                    case 3: _memberApp.MemberMenu(); break;
                    case 4: _membershipApp.MembershipMenu(); break;
                    case 5: _bookCopyApp.BookCopyMenu(); break;
                    case 6: _adminBorrowApp.BorrowManagementMenu(); break;
                    case 7: _fineRuleApp.FineRuleMenu(); break;
                    case 8: _reportsApp.ReportsMenu(); break;
                    case 9: 
                        _session.Logout(); 
                        ConsoleUi.WriteInfo("Logging out...");
                        ConsoleUi.WriteSuccess("Goodbye, Administrator! logged out successfully.");
                        ConsoleUi.Pause();
                        break;
                }
            }
        }

        private void RunUserMenu()
        {
            var member = _session.CurrentMember;
            ConsoleUi.WriteSuccess($"\nWelcome back, {member?.FullName}! Login successful.");
            ConsoleUi.Pause();

            while (_session.IsMember)
            {
                ConsoleUi.WriteTitle("User Menu");
                ConsoleUi.WriteMenuOptions(new[] { "View All Books", "View Books By Category", "Search Books", "Borrow / Return / Renew", "My Fines", "Logout" });

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 6);
                switch (choice)
                {
                    case 1: _bookApp.ViewBooks(); break;
                    case 2: _bookApp.ViewBooksByCategory(); break;
                    case 3: _bookApp.SearchBooks(); break;
                    case 4: _borrowApp.BorrowMenu(); break;
                    case 5: _fineManagementApp.FineMenu(); break;
                    case 6: 
                        _session.Logout();
                        ConsoleUi.WriteInfo("Logging out...");
                        ConsoleUi.WriteSuccess($"Goodbye, {member?.FullName}! Have a nice day.");
                        ConsoleUi.Pause();
                        break;
                }
            }
        }
    }
}