using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLendingApp.DALLibrary.Contexts
{
    public class BookLendingAppContext : DbContext
    {
        public DbSet<BookCategory> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<FineRule> FineRules { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=booklendingappdb;Username=postgres;Password=978681");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BookId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Author).HasMaxLength(200);
                entity.Property(e => e.ISBN).HasMaxLength(50);
                entity.Property(e => e.Publisher).HasMaxLength(200);
                entity.HasOne<BookCategory>()
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookCopy>(entity =>
            {
                entity.HasKey(e => e.BookCopyId);
                entity.Property(e => e.Barcode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ShelfLocation).HasMaxLength(100);
                entity.Property(e => e.Condition).HasMaxLength(200);
                entity.Property(e => e.DamagePercentage).HasPrecision(5, 2);
                entity.HasIndex(e => e.Barcode).IsUnique();
                entity.HasOne<Book>()
                      .WithMany()
                      .HasForeignKey(e => e.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasKey(e => e.BorrowRecordId);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.DamagePercentage).HasPrecision(5, 2);
                entity.HasOne<Member>()
                      .WithMany()
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Book>()
                      .WithMany()
                      .HasForeignKey(e => e.BookCopyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FineRule>(entity =>
            {
                entity.HasKey(e => e.FineAmountId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Percentage).HasPrecision(5, 2);
                entity.Property(e => e.FineType).HasConversion<int>();
                entity.Property(e => e.FineCalculationType).HasConversion<int>();
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberId);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EmailId).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MobileNumber).HasMaxLength(20);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.EmailId).IsUnique();
                entity.HasIndex(e => e.MobileNumber).IsUnique();
                entity.HasOne<MemberShip>()
                      .WithMany()
                      .HasForeignKey(e => e.MembershipId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MemberShip>(entity =>
            {
                entity.HasKey(e => e.MemberShipId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MembershipFee).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentType).HasConversion<int>();
                entity.HasOne<Member>()
                      .WithMany()
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}