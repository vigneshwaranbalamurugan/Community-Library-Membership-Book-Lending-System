using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IBorrowingService
    {
        BorrowRecord BorrowBook(Guid memberId, Guid bookCopyId);
        BorrowRecord ReturnBook(Guid borrowRecordId, DateTime returnDate, decimal damagePercentage = 0);
        BorrowRecord RenewBorrow(Guid borrowRecordId);
        List<AvailableBookByCategory> GetAvailableBooksByCategory(Guid categoryId);
        MemberBorrowingSummary GetMemberBorrowingSummary(Guid memberId);
        decimal GetUnpaidFine(Guid memberId);
        List<BorrowRecord> GetActiveBorrowRecords(Guid memberId);
        List<BorrowRecord> GetOverdueBorrowRecords();
        bool CanBorrow(Guid memberId, Guid bookId, out string validationMessage);
    }
}