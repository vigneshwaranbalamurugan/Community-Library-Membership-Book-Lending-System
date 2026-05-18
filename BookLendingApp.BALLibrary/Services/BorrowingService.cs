using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BookLendingApp.Ballibrary.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly BookLendingAppContext _context;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        private readonly IBorrowRecordRepository _borrowRecordRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IFineRuleRepository _fineRuleRepository;

        public BorrowingService(
            BookLendingAppContext context,
            IMemberRepository memberRepository,
            IMembershipRepository membershipRepository,
            IBookRepository bookRepository,
            IBookCopyRepository bookCopyRepository,
            IBorrowRecordRepository borrowRecordRepository,
            IPaymentRepository paymentRepository,
            IFineRuleRepository fineRuleRepository)
        {
            _context = context;
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _bookRepository = bookRepository;
            _bookCopyRepository = bookCopyRepository;
            _borrowRecordRepository = borrowRecordRepository;
            _paymentRepository = paymentRepository;
            _fineRuleRepository = fineRuleRepository;
        }

        public BorrowRecord BorrowBook(Guid memberId, Guid bookCopyId)
        {
            // Perform read-only validations first (avoid opening a transaction while running raw DB functions)
            var member = _memberRepository.Get(memberId) ?? throw new KeyNotFoundException($"Member {memberId} not found.");
                if (!member.IsActive)
                {
                    throw new InvalidOperationException("Member is inactive.");
                }
            var membership = _membershipRepository.Get(member.MembershipId) ?? throw new KeyNotFoundException($"Membership {member.MembershipId} not found.");

            var unpaidFine = GetUnpaidFine(memberId);
            if (unpaidFine > 500)
            {
                throw new InvalidOperationException("Unpaid fine exceeds the borrowing limit of ₹500.");
            }

            var bookCopy = _bookCopyRepository.Get(bookCopyId) ?? throw new KeyNotFoundException($"Book copy {bookCopyId} not found.");
            if (bookCopy.Status != BookStatus.Available && bookCopy.Status != BookStatus.Damaged)
            {
                throw new InvalidOperationException("Book copy is not available.");
            }

            var book = _bookRepository.Get(bookCopy.BookId) ?? throw new KeyNotFoundException($"Book {bookCopy.BookId} not found.");
            if (_borrowRecordRepository.HasActiveBorrowForBook(memberId, book.BookId))
            {
                throw new InvalidOperationException("Member already has an active borrow for this book.");
            }

            var activeBorrowCount = _borrowRecordRepository.GetActiveBorrowCount(memberId);
            if (activeBorrowCount >= membership.MaxBooksAllowed)
            {
                throw new InvalidOperationException("Borrowing limit exceeded.");
            }

            var dueDate = DateTime.UtcNow.AddDays(membership.MaxBorrowDurationDays);
            var borrowRecord = new BorrowRecord
            {
                BorrowRecordId = Guid.NewGuid(),
                MemberId = memberId,
                BookCopyId = bookCopyId,
                BorrowDate = DateTime.UtcNow,
                BorrowStatus = BorrowStatus.Active,
                RenewalCount = 0,
                RenewalDays = membership.MaxBorrowDurationDays
            };

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _borrowRecordRepository.Create(borrowRecord);
                bookCopy.Status = BookStatus.LentOut;
                _bookCopyRepository.Update(bookCopy.BookCopyId, bookCopy);

                transaction.Commit();
                return borrowRecord;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public BorrowRecord ReturnBook(Guid borrowRecordId, DateTime returnDate, decimal damagePercentage = 0)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var borrowRecord = _borrowRecordRepository.Get(borrowRecordId) ?? throw new KeyNotFoundException($"Borrow record {borrowRecordId} not found.");
                if (borrowRecord.BorrowStatus != BorrowStatus.Active)
                {
                    throw new InvalidOperationException("Borrow record is not active.");
                }

                var bookCopy = _bookCopyRepository.Get(borrowRecord.BookCopyId) ?? throw new KeyNotFoundException($"Book copy {borrowRecord.BookCopyId} not found.");
                var member = _memberRepository.Get(borrowRecord.MemberId) ?? throw new KeyNotFoundException($"Member {borrowRecord.MemberId} not found.");
                var membership = _membershipRepository.Get(member.MembershipId) ?? throw new KeyNotFoundException($"Membership {member.MembershipId} not found.");

                var dueDate = borrowRecord.BorrowDate.AddDays(borrowRecord.RenewalDays);
                var delayedDays = (int)Math.Max(0, (returnDate.Date - dueDate.Date).TotalDays);
                var fineAmount = CalculateLateFee(delayedDays);

                borrowRecord.ReturnDate = returnDate;
                borrowRecord.BorrowStatus = BorrowStatus.Returned;
                borrowRecord.DamagePercentage = damagePercentage;
                _borrowRecordRepository.Update(borrowRecordId, borrowRecord);

                bookCopy.Status = damagePercentage > 0 ? BookStatus.Damaged : BookStatus.Available;
                _bookCopyRepository.Update(bookCopy.BookCopyId, bookCopy);

                if (fineAmount > 0)
                {
                    _paymentRepository.Create(new LateFeePayment
                    {
                        PaymentId = Guid.NewGuid(),
                        MemberId = member.MemberId,
                        BorrowRecordId = borrowRecord.BorrowRecordId,
                        Amount = fineAmount,
                        PaymentDate = DateTime.UtcNow
                    });
                }

                transaction.Commit();
                return borrowRecord;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public BorrowRecord RenewBorrow(Guid borrowRecordId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var borrowRecord = _borrowRecordRepository.Get(borrowRecordId) ?? throw new KeyNotFoundException($"Borrow record {borrowRecordId} not found.");
                if (borrowRecord.BorrowStatus != BorrowStatus.Active)
                {
                    throw new InvalidOperationException("Only active borrows can be renewed.");
                }

                var member = _memberRepository.Get(borrowRecord.MemberId) ?? throw new KeyNotFoundException($"Member {borrowRecord.MemberId} not found.");
                var membership = _membershipRepository.Get(member.MembershipId) ?? throw new KeyNotFoundException($"Membership {member.MembershipId} not found.");

                if (!membership.IsRenewalAllowed)
                {
                    throw new InvalidOperationException("Renewal is not allowed for this membership.");
                }

                if (borrowRecord.RenewalCount >= membership.MaxRenewalTimes)
                {
                    throw new InvalidOperationException("Renewal limit exceeded.");
                }

                borrowRecord.RenewalCount += 1;
                borrowRecord.RenewalDays += membership.MaxRenewalDurationDays;
                borrowRecord.BorrowStatus = BorrowStatus.Renewed;

                _borrowRecordRepository.Update(borrowRecordId, borrowRecord);
                transaction.Commit();
                return borrowRecord;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public decimal GetUnpaidFine(Guid memberId)
        {
            try
            {
                return GetUnpaidFineFromDatabaseFunction(memberId);
            }
            catch
            {
                return _paymentRepository.GetUnpaidAmountByMember(memberId);
            }
        }

        public MemberBorrowingSummary GetMemberBorrowingSummary(Guid memberId)
        {
            var connection = _context.Database.GetDbConnection();
            var openedHere = false;
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT active_borrows, returned_borrows, total_unpaid_fine FROM get_member_borrowing_summary(@member_id)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@member_id";
                parameter.Value = memberId;
                command.Parameters.Add(parameter);

                using var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return new MemberBorrowingSummary();
                }

                return new MemberBorrowingSummary
                {
                    ActiveBorrows = Convert.ToInt64(reader["active_borrows"]),
                    ReturnedBorrows = Convert.ToInt64(reader["returned_borrows"]),
                    TotalUnpaidFine = reader["total_unpaid_fine"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["total_unpaid_fine"])
                };
            }
            finally
            {
                if (openedHere)
                {
                    connection.Close();
                }
            }
        }

        public List<AvailableBookByCategory> GetAvailableBooksByCategory(Guid categoryId)
        {
            var books = new List<AvailableBookByCategory>();
            var connection = _context.Database.GetDbConnection();
            var openedHere = false;
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT bookid, title, author, isbn, available_copies FROM get_available_books_by_category(@category_id)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@category_id";
                parameter.Value = categoryId;
                command.Parameters.Add(parameter);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new AvailableBookByCategory
                    {
                        BookId = reader["bookid"] == DBNull.Value ? Guid.Empty : (Guid)reader["bookid"],
                        Title = reader["title"]?.ToString() ?? string.Empty,
                        Author = reader["author"]?.ToString() ?? string.Empty,
                        Isbn = reader["isbn"]?.ToString() ?? string.Empty,
                        AvailableCopies = reader["available_copies"] == DBNull.Value ? 0L : Convert.ToInt64(reader["available_copies"])
                    });
                }

                return books;
            }
            finally
            {
                if (openedHere)
                {
                    connection.Close();
                }
            }
        }

        public List<BorrowRecord> GetActiveBorrowRecords(Guid memberId)
        {
            return _borrowRecordRepository.GetActiveBorrowRecordsByMember(memberId);
        }

        public List<BorrowRecord> GetOverdueBorrowRecords()
        {
            return _borrowRecordRepository.GetOverdueBorrowRecords();
        }

        public List<BorrowRecord> GetAllActiveBorrowRecords()
        {
            var all = _borrowRecordRepository.GetAll();
            return all.Where(r => r.BorrowStatus == BorrowStatus.Active).ToList();
        }

        public List<MostBorrowedBook> GetMostBorrowedBooks(int topN = 10)
        {
            var all = _borrowRecordRepository.GetAll();

            var grouped = all
                .Select(r => _bookCopyRepository.Get(r.BookCopyId))
                .Where(bc => bc != null)
                .Select(bc => bc!) // assert non-null after filter for the compiler
                .GroupBy(bc => bc.BookId)
                .Select(g => new { BookId = g.Key, Count = g.LongCount() })
                .OrderByDescending(x => x.Count)
                .Take(topN)
                .ToList();

            var result = new List<MostBorrowedBook>();
            foreach (var item in grouped)
            {
                var book = _bookRepository.Get(item.BookId);
                if (book == null) continue;
                result.Add(new MostBorrowedBook
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Isbn = book.ISBN,
                    BorrowCount = item.Count
                });
            }

            return result;
        }

        public List<MemberPendingFine> GetMembersWithPendingFines()
        {
            var members = _memberRepository.GetAll();
            var list = new List<MemberPendingFine>();
            foreach (var m in members)
            {
                var unpaid = GetUnpaidFine(m.MemberId);
                if (unpaid > 0)
                {
                    list.Add(new MemberPendingFine
                    {
                        MemberId = m.MemberId,
                        FullName = m.FullName,
                        EmailId = m.EmailId,
                        UnpaidAmount = unpaid
                    });
                }
            }

            return list.OrderByDescending(x => x.UnpaidAmount).ToList();
        }

        public List<BorrowRecord> GetBorrowHistoryByMember(Guid memberId)
        {
            return _borrowRecordRepository.GetBorrowRecordsByMember(memberId);
        }

        public bool CanBorrow(Guid memberId, Guid bookId, out string validationMessage)
        {
            validationMessage = string.Empty;
            var member = _memberRepository.Get(memberId);
            if (member == null)
            {
                validationMessage = "Member not found.";
                return false;
            }

            if (!member.IsActive)
            {
                validationMessage = "Member is inactive.";
                return false;
            }

            var membership = _membershipRepository.Get(member.MembershipId);
            if (membership == null)
            {
                validationMessage = "Membership not found.";
                return false;
            }

            if (GetUnpaidFine(memberId) > 500)
            {
                validationMessage = "Unpaid fine exceeds ₹500.";
                return false;
            }

            if (_borrowRecordRepository.GetActiveBorrowCount(memberId) >= membership.MaxBooksAllowed)
            {
                validationMessage = "Borrowing limit exceeded.";
                return false;
            }

            if (_borrowRecordRepository.HasActiveBorrowForBook(memberId, bookId))
            {
                validationMessage = "Member already has this book borrowed and not returned.";
                return false;
            }

            validationMessage = "Eligible to borrow.";
            return true;
        }

        private decimal GetUnpaidFineFromDatabaseFunction(Guid memberId)
        {
            var connection = _context.Database.GetDbConnection();
            var openedHere = false;
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT calculate_member_fine(@member_id)";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@member_id";
            parameter.Value = memberId;
            command.Parameters.Add(parameter);

            var result = command.ExecuteScalar();

            if (openedHere)
            {
                connection.Close();
            }

            return result == null || result == DBNull.Value ? 0m : Convert.ToDecimal(result);
        }

        private decimal CalculateLateFee(int delayedDays)
        {
            if (delayedDays <= 0)
            {
                return 0m;
            }

            var lateReturnRules = _fineRuleRepository.GetFineRulesByFineType(FineType.LateReturn);
            if (lateReturnRules.Count == 0)
            {
                return delayedDays * 10m;
            }

            var rule = lateReturnRules[0];
            return rule.FineCalculationType switch
            {
                FineCalculationType.PerDay => delayedDays * rule.Amount,
                FineCalculationType.FlatFee => rule.Amount,
                FineCalculationType.Percentage => rule.Amount * ((rule.Percentage ?? 100m) / 100m),
                _ => delayedDays * 10m
            };
        }
    }
}