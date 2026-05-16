using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.Ballibrary.Services;
using BookLendingApp.Application.Book;
using BookLendingApp.Application.Member;
using BookLendingApp.Application.Borrow;
using BookLendingApp.Application.Payment;
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

        private readonly IBookCategoryService _bookCategoryService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;
        private readonly IBookCopyService _bookCopyService;
        private readonly IFineRuleService _fineRuleService;

        private readonly BookCategoryApp _bookCategoryApp;
        private readonly BookApp _bookApp;
        private readonly MemberApp _memberApp;
        private readonly MembershipApp _membershipApp;
        private readonly BookCopyApp _bookCopyApp;
        private readonly FineRuleApp _fineRuleApp;

        Program()
        {
            _context = new BookLendingAppContext();

            _bookRepository = new BookRepository(_context);
            _bookCategoryRepository = new BookCategoryRepository(_context);
            _memberRepository = new MemberRepository(_context);
            _membershipRepository = new MembershipRepository(_context);
            _bookCopyRepository = new BookCopyRepository(_context);
            _fineRuleRepository = new FineRuleRepository(_context);

            _bookCategoryService = new BookCategoryService(_bookCategoryRepository);
            _bookService = new BookService(_bookRepository, _bookCategoryRepository);
            _memberService = new MemberService(_memberRepository);
            _membershipService = new MembershipService(_membershipRepository);
            _bookCopyService = new BookCopyService(_bookCopyRepository);
            _fineRuleService = new FineRuleService(_fineRuleRepository);

            _bookCategoryApp = new BookCategoryApp(_bookCategoryService);
            _bookApp = new BookApp(_bookService);
            _memberApp = new MemberApp(_memberService);
            _membershipApp = new MembershipApp(_membershipService);
            _bookCopyApp = new BookCopyApp(_bookCopyService);
            _fineRuleApp = new FineRuleApp(_fineRuleService);
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
                Console.WriteLine("6. Fine Rules");
                Console.WriteLine("7. Exit");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": _bookApp.BookMenu(); break;
                    case "2": _bookCategoryApp.CategoryMenu(); break;
                    case "3": _memberApp.MemberMenu(); break;
                    case "4": _membershipApp.MembershipMenu(); break;
                    case "5": _bookCopyApp.BookCopyMenu(); break;
                    case "6": _fineRuleApp.FineRuleMenu(); break;
                    case "7": return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}