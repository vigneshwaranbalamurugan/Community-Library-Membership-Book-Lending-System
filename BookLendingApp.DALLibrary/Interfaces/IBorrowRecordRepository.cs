using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IBorrowRecordRepository : IRepository<Guid, BorrowRecord>
    {
        List<BorrowRecord> GetActiveBorrowRecordsByMember(Guid memberId);
        List<BorrowRecord> GetBorrowRecordsByMember(Guid memberId);
        List<BorrowRecord> GetBorrowRecordsByBook(Guid bookId);
        BorrowRecord? GetActiveBorrowRecordByCopy(Guid bookCopyId);
        bool HasActiveBorrowForBook(Guid memberId, Guid bookId);
        int GetActiveBorrowCount(Guid memberId);
        List<BorrowRecord> GetOverdueBorrowRecords();
    }
}