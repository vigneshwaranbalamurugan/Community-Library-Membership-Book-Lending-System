using System.ComponentModel.DataAnnotations;

namespace BookLendingApp.ModelLibrary.Models
{
    public class Book
    {
        [Key]
        public Guid BookId { get; set; }

        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [Required]
        [StringLength(200)]
        public required string Author { get; set; }

        [Required]
        [StringLength(20)]
        public required string ISBN { get; set; }

        [Range(1, 9999)]
        public int PublicationYear { get; set; }

        [Required]
        [StringLength(200)]
        public required string Publisher { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public BookCategory? Category { get; set; }

        public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Book()
        {
            
        }

        public Book(string title, string author, string isbn, int publicationYear, string publisher, Guid categoryId)
        {
            BookId = Guid.NewGuid();
            Title = title;
            Author = author;
            ISBN = isbn;
            PublicationYear = publicationYear;
            Publisher = publisher;
            CategoryId = Guid.Parse(categoryId.ToString());
        }
    }
}