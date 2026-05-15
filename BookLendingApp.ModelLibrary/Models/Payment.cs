using BookLendingApp.ModelLibrary.Enums;

namespace BookLendingApp.ModelLibrary.Models
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid MemberId { get; set; }
        public Guid? BorrowRecordId { get; set; }
        public Decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }

    public class MembershipPayment : Payment
    {
        public MembershipPayment()
        {
            PaymentType = PaymentType.MembershipFee;
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
        }
    }

    public class OtherPayment : Payment
    {
        public OtherPayment()
        {
            PaymentType = PaymentType.Other;
        }
    }
}