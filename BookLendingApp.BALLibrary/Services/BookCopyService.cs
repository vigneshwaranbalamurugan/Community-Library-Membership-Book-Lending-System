using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly IBookCopyRepository _bookCopyRepository;

        public BookCopyService(IBookCopyRepository bookCopyRepository)
        {
            _bookCopyRepository = bookCopyRepository;
        }

        public BookCopy AddBookCopy(Guid bookId, string barcode, string shelfLocation, BookStatus status = BookStatus.Available, decimal damagePercentage = 0, string? condition = null)
        {
            var bookCopy = new BookCopy
            {
                BookId = bookId,
                Barcode = barcode,
                ShelfLocation = shelfLocation,
                Status = status,
                DamagePercentage = damagePercentage,
                Condition = condition
            };
            return _bookCopyRepository.Create(bookCopy);
        }

        public BookCopy UpdateBookCopy(Guid bookCopyId, Guid bookId, string barcode, string shelfLocation, BookStatus status, decimal damagePercentage = 0, string? condition = null)
        {
            var updated = new BookCopy
            {
                BookCopyId = bookCopyId,
                BookId = bookId,
                Barcode = barcode,
                ShelfLocation = shelfLocation,
                Status = status,
                DamagePercentage = damagePercentage,
                Condition = condition
            };
            return _bookCopyRepository.Update(bookCopyId, updated);
        }

        public void RemoveBookCopy(Guid bookCopyId) => _bookCopyRepository.Delete(bookCopyId);

        public BookCopy GetBookCopyById(Guid bookCopyId)
        {
            return _bookCopyRepository.Get(bookCopyId) ?? throw new KeyNotFoundException($"BookCopy {bookCopyId} not found.");
        }

        public List<BookCopy> GetAllBookCopies() => _bookCopyRepository.GetAll();

        public List<BookCopy> GetCopiesByBookId(Guid bookId) => _bookCopyRepository.GetCopiesByBookId(bookId);

        public List<BookCopy> GetAvailableCopiesByBookId(Guid bookId) => _bookCopyRepository.GetAvailableCopiesByBookId(bookId);

        public List<BookCopy> GetCopiesByStatus(Guid bookId, BookStatus status) => _bookCopyRepository.GetCopiesByStatus(bookId, status);

        public List<BookCopy> GetCopiesByCondition(Guid bookId, string condition) => _bookCopyRepository.GetCopiesByCondition(bookId, condition);
    }
}