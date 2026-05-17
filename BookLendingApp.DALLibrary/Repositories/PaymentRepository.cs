using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Repositories
{
	public class PaymentRepository : AbstractRepository<Guid, Payment>, IPaymentRepository
	{
		public PaymentRepository(BookLendingAppContext context) : base(context)
		{
		}

		public override Payment Create(Payment item)
		{
			_context.Payments.Add(item);
			_context.SaveChanges();
			return item;
		}

		public override Payment Update(Guid key, Payment item)
		{
			var existing = _context.Payments.FirstOrDefault(p => p.PaymentId == key);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Payment with ID {key} not found.");
			}

			existing.MemberId = item.MemberId;
			existing.BorrowRecordId = item.BorrowRecordId;
			existing.Amount = item.Amount;
			existing.PaymentType = item.PaymentType;
			existing.IsPaid = item.IsPaid;
			existing.PaymentDate = item.PaymentDate;

			_context.SaveChanges();
			return existing;
		}

		public override Payment? Get(Guid key)
		{
			return _context.Payments.FirstOrDefault(p => p.PaymentId == key);
		}

		public override List<Payment> GetAll()
		{
			return _context.Payments.ToList();
		}

		public override Payment Delete(Guid key)
		{
			var existing = _context.Payments.FirstOrDefault(p => p.PaymentId == key);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Payment with ID {key} not found.");
			}

			_context.Payments.Remove(existing);
			_context.SaveChanges();
			return existing;
		}

		public List<Payment> GetPaymentsByMember(Guid memberId)
		{
			return _context.Payments.Where(p => p.MemberId == memberId).ToList();
		}

		public List<Payment> GetPaymentsByBorrowRecord(Guid borrowRecordId)
		{
			return _context.Payments.Where(p => p.BorrowRecordId == borrowRecordId).ToList();
		}

		public List<Payment> GetUnpaidPaymentsByMember(Guid memberId)
		{
			return _context.Payments.Where(p => p.MemberId == memberId && !p.IsPaid).ToList();
		}

		public decimal GetUnpaidAmountByMember(Guid memberId)
		{
			return _context.Payments
				.Where(p => p.MemberId == memberId && !p.IsPaid)
				.Sum(p => p.Amount);
		}

		public decimal GetTotalPaidByBorrowRecord(Guid borrowRecordId)
		{
			return _context.Payments.Where(p => p.BorrowRecordId == borrowRecordId).Sum(p => p.Amount);
		}

		public bool MarkPaymentPaid(Guid paymentId)
		{
			var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
			if (payment == null)
			{
				return false;
			}

			payment.IsPaid = true;
			_context.SaveChanges();
			return true;
		}
	}
}
