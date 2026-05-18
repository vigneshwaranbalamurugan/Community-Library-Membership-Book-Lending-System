using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.ModelLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class BookCopyRepository: AbstractRepository<Guid, BookCopy>, IBookCopyRepository
    {
        public BookCopyRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override BookCopy Create(BookCopy item)
        {
            _context.BookCopies.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override BookCopy Update(Guid key, BookCopy item)
        {
            var existingCopy = _context.BookCopies.FirstOrDefault(b => b.BookCopyId == key);
            if (existingCopy == null)
                throw new KeyNotFoundException($"BookCopy with ID {key} not found.");
            
            existingCopy.BookId = item.BookId;
            existingCopy.Barcode = item.Barcode;
            existingCopy.ShelfLocation = item.ShelfLocation;
            existingCopy.Status = item.Status;
            existingCopy.DamagePercentage = item.DamagePercentage;
            existingCopy.Condition = item.Condition;
            
            _context.SaveChanges();
            return existingCopy;
        }
        public override BookCopy? Get(Guid key)
        {
            return _context.BookCopies.Include(b => b.Book).FirstOrDefault(b => b.BookCopyId == key);
        }
        public override List<BookCopy> GetAll()
        {
            return _context.BookCopies.Include(b => b.Book).ToList();
        }
        public override BookCopy Delete(Guid key)
        {
            var copy = _context.BookCopies.FirstOrDefault(b => b.BookCopyId == key);
            if (copy == null)
                throw new KeyNotFoundException($"BookCopy with ID {key} not found.");
            
            _context.BookCopies.Remove(copy);
            _context.SaveChanges();
            return copy;
        }

        public List<BookCopy> GetCopiesByBookId(Guid bookId)
        {
            return _context.BookCopies.Where(c => c.BookId == bookId).Include(c => c.Book).ToList();
        }

        public List<BookCopy> GetAvailableCopiesByBookId(Guid bookId)
        {
            return _context.BookCopies
                .Where(c => c.BookId == bookId && c.Status == BookStatus.Available)
                .Include(c => c.Book)
                .ToList();
        }

        public List<BookCopy> GetCopiesByStatus(Guid bookId, BookStatus status)
        {
            return _context.BookCopies
                .Where(c => c.BookId == bookId && c.Status == status)
                .Include(c => c.Book)
                .ToList();
        }

        public List<BookCopy> GetCopiesByCondition(Guid bookId, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return new List<BookCopy>();
            var term = $"%{condition}%";
            return _context.BookCopies
                .Where(c => c.BookId == bookId && c.Condition != null && EF.Functions.ILike(c.Condition, term))
                .Include(c => c.Book)
                .ToList();
        }
    }
}