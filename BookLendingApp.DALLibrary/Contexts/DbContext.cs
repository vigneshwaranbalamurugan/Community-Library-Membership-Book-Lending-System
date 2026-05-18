using BookLendingApp.ModelLibrary.Enums;
using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
                entity.HasKey(category => category.CategoryId);
                entity.Property(category => category.Name).IsRequired().HasMaxLength(50);
                entity.Property(category => category.Description).HasMaxLength(200);
                entity.HasMany(category => category.Books)
                    .WithOne(book => book.Category)
                    .HasForeignKey(book => book.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(book => book.BookId);
                entity.Property(book => book.Title).IsRequired().HasMaxLength(200);
                entity.Property(book => book.Author).IsRequired().HasMaxLength(200);
                entity.Property(book => book.ISBN).IsRequired().HasMaxLength(20);
                entity.Property(book => book.Publisher).IsRequired().HasMaxLength(200);
                entity.HasIndex(book => book.ISBN).IsUnique();
                entity.HasMany(book => book.BookCopies)
                    .WithOne(copy => copy.Book)
                    .HasForeignKey(copy => copy.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookCopy>(entity =>
            {
                entity.HasKey(copy => copy.BookCopyId);
                entity.Property(copy => copy.Barcode).IsRequired().HasMaxLength(100);
                entity.Property(copy => copy.ShelfLocation).IsRequired().HasMaxLength(100);
                entity.Property(copy => copy.Condition).HasMaxLength(500);
                entity.Property(copy => copy.DamagePercentage).HasPrecision(5, 2);
                entity.HasIndex(copy => copy.Barcode).IsUnique();
                entity.HasMany(copy => copy.BorrowRecords)
                    .WithOne(record => record.BookCopy)
                    .HasForeignKey(record => record.BookCopyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasKey(record => record.BorrowRecordId);
                entity.Property(record => record.Notes).HasMaxLength(1000);
                entity.Property(record => record.DamagePercentage).HasPrecision(5, 2);
                entity.HasOne(record => record.Member)
                    .WithMany(member => member.BorrowRecords)
                    .HasForeignKey(record => record.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(record => record.Payments)
                    .WithOne(payment => payment.BorrowRecord)
                    .HasForeignKey(payment => payment.BorrowRecordId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<FineRule>(entity =>
            {
                entity.HasKey(rule => rule.FineAmountId);
                entity.Property(rule => rule.Name).IsRequired().HasMaxLength(100);
                entity.Property(rule => rule.Description).HasMaxLength(500);
                entity.Property(rule => rule.Percentage).HasPrecision(5, 2);
                entity.Property(rule => rule.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<MemberShip>(entity =>
            {
                entity.HasKey(membership => membership.MemberShipId);
                entity.Property(membership => membership.Name).IsRequired().HasMaxLength(100);
                entity.Property(membership => membership.MembershipFee).HasPrecision(18, 2);
                entity.HasMany(membership => membership.Members)
                    .WithOne(member => member.Membership)
                    .HasForeignKey(member => member.MembershipId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(member => member.MemberId);
                entity.Property(member => member.FullName).IsRequired().HasMaxLength(200);
                entity.Property(member => member.EmailId).IsRequired().HasMaxLength(254);
                entity.Property(member => member.MobileNumber).HasMaxLength(20);
                entity.Property(member => member.Password).IsRequired().HasMaxLength(255);
                entity.HasIndex(member => member.EmailId).IsUnique();
                entity.HasIndex(member => member.MobileNumber).IsUnique();
                entity.HasMany(member => member.Payments)
                    .WithOne(payment => payment.Member)
                    .HasForeignKey(payment => payment.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(payment => payment.PaymentId);
                entity.Property(payment => payment.Amount).HasPrecision(18, 2);
                entity.Property<string>("PaymentDiscriminator")
                    .HasMaxLength(50)
                    .HasColumnName("PaymentDiscriminator");
                entity.HasDiscriminator<string>("PaymentDiscriminator")
                    .HasValue<Payment>(nameof(Payment))
                    .HasValue<MembershipPayment>(nameof(MembershipPayment))
                    .HasValue<LateFeePayment>(nameof(LateFeePayment))
                    .HasValue<BookDamageFeePayment>(nameof(BookDamageFeePayment))
                    .HasValue<LostBookFeePayment>(nameof(LostBookFeePayment))
                    .HasValue<RenewalFeePayment>(nameof(RenewalFeePayment))
                    .HasValue<OtherPayment>(nameof(OtherPayment));
            });
        }

        public override int SaveChanges()
        {
            ApplyAuditTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditTimestamps()
        {
            var utcNow = DateTime.UtcNow;

            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    SetAuditTimestamps(entry.Entity, utcNow, created: true);
                }
                else if (entry.State == EntityState.Modified)
                {
                    SetAuditTimestamps(entry.Entity, utcNow, created: false);
                }
            }
        }

        private static void SetAuditTimestamps(object entity, DateTime utcNow, bool created)
        {
            switch (entity)
            {
                case Book book:
                    if (created)
                    {
                        book.CreatedAt = utcNow;
                    }

                    book.UpdatedAt = utcNow;
                    break;
                case BookCategory category:
                    if (created)
                    {
                        category.CreatedAt = utcNow;
                    }

                    category.UpdatedAt = utcNow;
                    break;
                case BookCopy bookCopy:
                    if (created)
                    {
                        bookCopy.CreatedAt = utcNow;
                    }

                    bookCopy.UpdatedAt = utcNow;
                    break;
                case BorrowRecord borrowRecord:
                    if (created)
                    {
                        borrowRecord.CreatedAt = utcNow;
                    }

                    borrowRecord.UpdatedAt = utcNow;
                    break;
                case FineRule fineRule:
                    if (created)
                    {
                        fineRule.CreatedAt = utcNow;
                    }

                    fineRule.UpdatedAt = utcNow;
                    break;
                case Member member:
                    if (created)
                    {
                        member.CreatedAt = utcNow;
                    }

                    member.UpdatedAt = utcNow;
                    break;
                case MemberShip membership:
                    if (created)
                    {
                        membership.CreatedAt = utcNow;
                    }

                    membership.UpdatedAt = utcNow;
                    break;
                case Payment payment:
                    if (created)
                    {
                        payment.CreatedAt = utcNow;
                    }
                    payment.UpdatedAt = utcNow;
                    break;
                default:
                    // If the entity doesn't have CreatedAt/UpdatedAt properties, do nothing
                    break;
            }
        }
    }
}