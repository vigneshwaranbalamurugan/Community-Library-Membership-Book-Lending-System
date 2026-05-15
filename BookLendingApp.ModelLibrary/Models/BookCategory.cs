using System.ComponentModel.DataAnnotations;

namespace BookLendingApp.ModelLibrary.Models
{
    public class BookCategory
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public BookCategory()
        {
            
        }

        public BookCategory(string name, string? description = null)
        {
            CategoryId = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}