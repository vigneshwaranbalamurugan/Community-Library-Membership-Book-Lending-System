using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class BookRepository: AbstractRepository<Guid, Book>, IBookRepository
    {
        public BookRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override Book Create(Book item)
        {
            _context.Books.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override Book Update(Guid key, Book item)
        {
            var existingBook = _context.Books.FirstOrDefault(b => b.BookId == key);
            if (existingBook == null)
                throw new KeyNotFoundException($"Book with ID {key} not found.");
            
            existingBook.Title = item.Title;
            existingBook.Author = item.Author;
            existingBook.ISBN = item.ISBN;
            existingBook.PublicationYear = item.PublicationYear;
            existingBook.Publisher = item.Publisher;
            existingBook.CategoryId = item.CategoryId;
            
            _context.SaveChanges();
            return existingBook;
        }
        public override Book? Get(Guid key)
        {
            return _context.Books.FirstOrDefault(b => b.BookId == key);
        }
        public override List<Book> GetAll()
        {
            return _context.Books.ToList();
        }

        public List<Book> GetBooksByAuthor(string author)
        {
            return _context.Books
                .Where(b => EF.Functions.ILike(b.Author, $"%{author}%"))
                .ToList();
        }

        public List<Book> GetBooksByPublicationYear(int year)
        {
            return _context.Books.Where(b => b.PublicationYear == year).ToList();
        }

        public List<Book> GetBooksByCategory(Guid categoryId)
        {
            return _context.Books.Where(b => b.CategoryId == categoryId).ToList();
        }

        public List<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<Book>();
            var term = $"%{searchTerm}%";
            return _context.Books
                .Where(b => EF.Functions.ILike(b.Title, term)
                         || EF.Functions.ILike(b.Author, term)
                         || EF.Functions.ILike(b.ISBN, term)
                         || EF.Functions.ILike(b.Publisher, term))
                .ToList();
        }
        public override Book Delete(Guid key)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == key);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {key} not found.");
            
            _context.Books.Remove(book);
            _context.SaveChanges();
            return book;
        }

        public Book GetBookByISBN(string isbn)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null)
                throw new KeyNotFoundException($"Book with ISBN {isbn} not found.");
            return book;
        }
    }
}