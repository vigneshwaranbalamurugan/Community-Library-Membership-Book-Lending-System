using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IBookService
    {
        void AddBook(string title, string author, int publicationYear, string publisher, string categoryName, string isbn);
        void UpdateBook(string isbn,string newTitle, string newAuthor, int newPublicationYear, string newPublisher, string newCategoryName);
        void RemoveBook(string isbn);
        Book GetBookById(Guid bookId);
        List<Book> GetAllBooks();
    }
}