using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Ballibrary.Services
{

    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookCategoryRepository _bookCategoryRepository;
        public BookService(IBookRepository bookRepository,IBookCategoryRepository bookCategoryRepository)
        {
            _bookRepository = bookRepository;
            _bookCategoryRepository = bookCategoryRepository;
        }
        public void AddBook(string title, string author, int publicationYear, string publisher, string categoryName, string isbn)
        {
            Book newBook = new Book
            {
                Title = title,
                Author = author,
                PublicationYear = publicationYear,
                Publisher = publisher,
                ISBN = isbn,
                CategoryId = _bookCategoryRepository.GetCategoryIdByName(categoryName)
            };
            _bookRepository.Create(newBook);
        }

        public void UpdateBook(string isbn,string newTitle, string newAuthor, int newPublicationYear, string newPublisher, string newCategoryName )
        {
            var existingBook = _bookRepository.GetBookByISBN(isbn);
            if (existingBook == null)
            {
                throw new Exception($"Book with ISBN {isbn} not found.");
            }

            existingBook.Title = newTitle;
            existingBook.Author = newAuthor;
            existingBook.PublicationYear = newPublicationYear;
            existingBook.Publisher = newPublisher;
            existingBook.CategoryId = _bookCategoryRepository.GetCategoryIdByName(newCategoryName);

            _bookRepository.Update(existingBook.BookId, existingBook);
        }

        public void RemoveBook(string isbn)
        {
            var book = _bookRepository.GetBookByISBN(isbn);
            if (book == null)
            {
                throw new Exception($"Book with ISBN {isbn} not found.");
            }
            _bookRepository.Delete(book.BookId);
        }

        public Book GetBookById(Guid bookId)
        {
            return _bookRepository.Get(bookId) ?? throw new Exception($"Book with ID {bookId} not found."); 
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll()??throw new Exception("No books found."); 
        }
    }
}
