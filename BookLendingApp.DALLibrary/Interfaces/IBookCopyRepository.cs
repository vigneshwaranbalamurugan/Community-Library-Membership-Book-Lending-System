using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IBookCopyRepository : IRepository<Guid, BookCopy>
    {
        List<BookCopy> GetCopiesByBookId(Guid bookId);
        List<BookCopy> GetAvailableCopiesByBookId(Guid bookId);
        List<BookCopy> GetCopiesByStatus(Guid bookId, BookStatus status);
        List<BookCopy> GetCopiesByCondition(Guid bookId, string condition);
               
    }
}