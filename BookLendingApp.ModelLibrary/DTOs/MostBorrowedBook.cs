using System;

namespace BookLendingApp.ModelLibrary.Models
{
    public class MostBorrowedBook
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public long BorrowCount { get; set; }
    }
}
