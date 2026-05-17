using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Repositories
{
	public class BorrowRecordRepository : AbstractRepository<Guid, BorrowRecord>, IBorrowRecordRepository
	{
		public BorrowRecordRepository(BookLendingAppContext context) : base(context)
		{
		}

		public override BorrowRecord Create(BorrowRecord item)
		{
			_context.BorrowRecords.Add(item);
			_context.SaveChanges();
			return item;
		}

		public override BorrowRecord Update(Guid key, BorrowRecord item)
		{
			var existing = _context.BorrowRecords.FirstOrDefault(r => r.BorrowRecordId == key);
			if (existing == null)
			{
				throw new KeyNotFoundException($"BorrowRecord with ID {key} not found.");
			}

			existing.MemberId = item.MemberId;
			existing.BookCopyId = item.BookCopyId;
			existing.BorrowDate = item.BorrowDate;
			existing.ReturnDate = item.ReturnDate;
			existing.RenewalCount = item.RenewalCount;
			existing.RenewalDays = item.RenewalDays;
			existing.BorrowStatus = item.BorrowStatus;
			existing.Notes = item.Notes;
			existing.DamagePercentage = item.DamagePercentage;

			_context.SaveChanges();
			return existing;
		}

		public override BorrowRecord? Get(Guid key)
		{
			return _context.BorrowRecords.FirstOrDefault(r => r.BorrowRecordId == key);
		}

		public override List<BorrowRecord> GetAll()
		{
			return _context.BorrowRecords.ToList();
		}

		public override BorrowRecord Delete(Guid key)
		{
			var existing = _context.BorrowRecords.FirstOrDefault(r => r.BorrowRecordId == key);
			if (existing == null)
			{
				throw new KeyNotFoundException($"BorrowRecord with ID {key} not found.");
			}

			_context.BorrowRecords.Remove(existing);
			_context.SaveChanges();
			return existing;
		}

		public List<BorrowRecord> GetActiveBorrowRecordsByMember(Guid memberId)
		{
			return _context.BorrowRecords.Where(r => r.MemberId == memberId && r.BorrowStatus == BorrowStatus.Active).ToList();
		}

		public List<BorrowRecord> GetBorrowRecordsByMember(Guid memberId)
		{
			return _context.BorrowRecords.Where(r => r.MemberId == memberId).ToList();
		}

		public List<BorrowRecord> GetBorrowRecordsByBook(Guid bookId)
		{
			return _context.BorrowRecords
				.Where(r => r.BookCopy != null && r.BookCopy.BookId == bookId)
				.ToList();
		}

		public BorrowRecord? GetActiveBorrowRecordByCopy(Guid bookCopyId)
		{
			return _context.BorrowRecords.FirstOrDefault(r => r.BookCopyId == bookCopyId && r.BorrowStatus == BorrowStatus.Active);
		}

		public bool HasActiveBorrowForBook(Guid memberId, Guid bookId)
		{
			return _context.BorrowRecords.Any(r => r.MemberId == memberId && r.BorrowStatus == BorrowStatus.Active && r.BookCopy != null && r.BookCopy.BookId == bookId);
		}

		public int GetActiveBorrowCount(Guid memberId)
		{
			return _context.BorrowRecords.Count(r => r.MemberId == memberId && r.BorrowStatus == BorrowStatus.Active);
		}

		public List<BorrowRecord> GetOverdueBorrowRecords()
		{
			var today = DateTime.UtcNow;
			return _context.BorrowRecords
				.Where(r => r.BorrowStatus == BorrowStatus.Active && r.BorrowDate.AddDays(r.RenewalDays > 0 ? r.RenewalDays : 0) < today)
				.ToList();
		}
	}
}
