using System.ComponentModel.DataAnnotations;
using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        public Guid? BorrowRecordId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999999.99")]
        public Decimal Amount { get; set; }

        [Range(1, int.MaxValue)]
        public PaymentType PaymentType { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public Member? Member { get; set; }

        public BorrowRecord? BorrowRecord { get; set; }
    }

    public class MembershipPayment : Payment
    {
        public MembershipPayment()
        {
            PaymentType = PaymentType.MembershipFee;
            IsPaid = true;
        }
    }

    public class LateFeePayment : Payment
    {
        public LateFeePayment()
        {
            PaymentType = PaymentType.LateFee;
        }
    }

    public class BookDamageFeePayment : Payment
    {
        public BookDamageFeePayment()
        {
            PaymentType = PaymentType.BookDamageFee;
        }
    }

    public class LostBookFeePayment : Payment
    {
        public LostBookFeePayment()
        {
            PaymentType = PaymentType.LostBookFee;
        }
    }
    public class RenewalFeePayment : Payment
    {
        public RenewalFeePayment()
        {
            PaymentType = PaymentType.RenewalFee;
            IsPaid = true;
        }
    }

    public class OtherPayment : Payment
    {
        public OtherPayment()
        {
            PaymentType = PaymentType.Other;
            IsPaid = true;
        }
    }
}