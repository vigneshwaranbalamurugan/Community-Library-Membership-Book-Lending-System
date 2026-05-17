using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.Ballibrary.Services;
using BookLendingApp.Application.Book;
using BookLendingApp.Application.Member;
using BookLendingApp.Application.Borrow;
using BookLendingApp.Application.Payment;
using BookLendingApp.Application.Admin;
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

        private readonly BookCategoryApp _bookCategoryApp;
        private readonly BookApp _bookApp;
        private readonly MemberApp _memberApp;
        private readonly MembershipApp _membershipApp;
        private readonly BookCopyApp _bookCopyApp;
        private readonly BorrowApp _borrowApp;
        private readonly FineManagementApp _fineManagementApp;
        private readonly FineRuleApp _fineRuleApp;
        private readonly ReportsApp _reportsApp;

        Program()
        {
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
            _borrowingService = new BorrowingService(_context, _memberRepository, _membershipRepository, _bookRepository, _bookCopyRepository, _borrowRecordRepository, _paymentRepository);
            _fineManagementService = new FineManagementService(_paymentRepository);

            _bookCategoryApp = new BookCategoryApp(_bookCategoryService);
            _bookApp = new BookApp(_bookService);
            _memberApp = new MemberApp(_memberService);
            _membershipApp = new MembershipApp(_membershipService);
            _bookCopyApp = new BookCopyApp(_bookCopyService);
            _borrowApp = new BorrowApp(_borrowingService);
            _fineManagementApp = new FineManagementApp(_fineManagementService);
            _fineRuleApp = new FineRuleApp(_fineRuleService);
            _reportsApp = new ReportsApp(_borrowingService);
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
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Books");
                Console.WriteLine("2. Categories");
                Console.WriteLine("3. Members");
                Console.WriteLine("4. Memberships");
                Console.WriteLine("5. Book Copies");
                Console.WriteLine("6. Borrow / Return / Renew");
                Console.WriteLine("7. Fine Management");
                Console.WriteLine("8. Fine Rules");
                Console.WriteLine("9. Reports");
                Console.WriteLine("10. Exit");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": _bookApp.BookMenu(); break;
                    case "2": _bookCategoryApp.CategoryMenu(); break;
                    case "3": _memberApp.MemberMenu(); break;
                    case "4": _membershipApp.MembershipMenu(); break;
                    case "5": _bookCopyApp.BookCopyMenu(); break;
                    case "6": _borrowApp.BorrowMenu(); break;
                    case "7": _fineManagementApp.FineMenu(); break;
                    case "8": _fineRuleApp.FineRuleMenu(); break;
                    case "9": _reportsApp.ReportsMenu(); break;
                    case "10": return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}