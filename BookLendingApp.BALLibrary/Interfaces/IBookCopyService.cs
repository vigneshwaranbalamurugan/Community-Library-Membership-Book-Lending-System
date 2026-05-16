using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IBookCopyService
    {
        BookCopy AddBookCopy(Guid bookId, string barcode, string shelfLocation, BookStatus status = BookStatus.Available, decimal damagePercentage = 0, string? condition = null);
        BookCopy UpdateBookCopy(Guid bookCopyId, Guid bookId, string barcode, string shelfLocation, BookStatus status, decimal damagePercentage = 0, string? condition = null);
        void RemoveBookCopy(Guid bookCopyId);
        BookCopy GetBookCopyById(Guid bookCopyId);
        List<BookCopy> GetAllBookCopies();
        List<BookCopy> GetCopiesByBookId(Guid bookId);
        List<BookCopy> GetAvailableCopiesByBookId(Guid bookId);
        List<BookCopy> GetCopiesByStatus(Guid bookId, BookStatus status);
        List<BookCopy> GetCopiesByCondition(Guid bookId, string condition);
    }
}