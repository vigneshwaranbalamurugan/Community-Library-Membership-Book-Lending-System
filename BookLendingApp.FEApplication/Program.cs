using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.Ballibrary.Services;
using BookLendingApp.Application.Book;
using BookLendingApp.Application.Member;
using BookLendingApp.Application.Borrow;
using BookLendingApp.Application.Payment;
using BookLendingApp.Application.Admin;
using BookLendingApp.FEApplication.Validation;
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
            _bookCopyApp = new BookCopyApp(_bookService, _bookCopyService);
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
            while (true)
            {
                Console.WriteLine("Authentication:");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Exit");

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
                Console.WriteLine("Invalid admin credentials.");
                return false;
            }

            _session.LoginAsAdmin();
            Console.WriteLine("Admin login successful.");
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
                Console.WriteLine($"Welcome, {member.FullName}.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        private void RunAdminMenu()
        {
            while (_session.IsAdmin)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Books");
                Console.WriteLine("2. Categories");
                Console.WriteLine("3. Members");
                Console.WriteLine("4. Memberships");
                Console.WriteLine("5. Book Copies");
                Console.WriteLine("6. Borrow Management");
                Console.WriteLine("7. Fine Rules");
                Console.WriteLine("8. Reports");
                Console.WriteLine("9. Logout");

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
                    case 9: _session.Logout(); break;
                }
            }
        }

        private void RunUserMenu()
        {
            while (_session.IsMember)
            {
                Console.WriteLine("User Menu:");
                Console.WriteLine("1. View Books");
                Console.WriteLine("2. Borrow / Return / Renew");
                Console.WriteLine("3. My Fines");
                Console.WriteLine("4. Logout");

                var choice = ConsoleInputValidator.ReadInt("Select an option:", 1, 4);
                switch (choice)
                {
                    case 1: _bookApp.ViewBooks(); break;
                    case 2: _borrowApp.BorrowMenu(); break;
                    case 3: _fineManagementApp.FineMenu(); break;
                    case 4: _session.Logout(); break;
                }
            }
        }
    }
}