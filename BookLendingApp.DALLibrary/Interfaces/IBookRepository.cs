using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IBookRepository : IRepository<Guid, Book>
    {
        List<Book> GetBooksByAuthor(string author);
        List<Book> GetBooksByPublicationYear(int year);
        List<Book> GetBooksByCategory(Guid categoryId);
        List<Book> SearchBooks(string searchTerm);  
    }
}