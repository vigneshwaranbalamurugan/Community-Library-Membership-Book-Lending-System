using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Application.Book
{
    public class BookApp
    {
       private readonly IBookService _bookService;

       public BookApp(IBookService bookService)
       {
           _bookService = bookService;
       }

       public void BookMenu()
       {
           Console.WriteLine("Book Menu:");
           Console.WriteLine("1. Add Book");
           Console.WriteLine("2. View Books");
           Console.WriteLine("3. Update Book");
           Console.WriteLine("4. Delete Book");
           Console.WriteLine("5. Back to Main Menu");

           var choice = Console.ReadLine();

           switch (choice)
           {
               case "1":
                   AddBook();
                   break;
               case "2":
                   ViewBooks();
                   break;
               case "3":
                   UpdateBook();
                   break;
               case "4":
                   DeleteBook();
                   break;
               case "5":
                   return;
               default:
                   Console.WriteLine("Invalid choice. Please try again.");
                   break;
           }
       }

        private void AddBook()
        {
            Console.WriteLine("Enter book title:");
            var title = Console.ReadLine();

            Console.WriteLine("Enter book author:");
            var author = Console.ReadLine();

            Console.WriteLine("Enter publication year (numbers only):");
            var yearInput = Console.ReadLine();
            int publicationYear = 0;
            if (!int.TryParse(yearInput, out publicationYear))
            {
                Console.WriteLine("Invalid year entered, defaulting to 0.");
            }

            Console.WriteLine("Enter publisher:");
            var publisher = Console.ReadLine();

            Console.WriteLine("Enter category name:");
            var categoryName = Console.ReadLine();

            Console.WriteLine("Enter ISBN:");
            var isbn = Console.ReadLine();

            _bookService.AddBook(title ?? string.Empty, author ?? string.Empty, publicationYear, publisher ?? string.Empty, categoryName ?? string.Empty, isbn ?? string.Empty);
            Console.WriteLine("Book added successfully.");
        }
        
        private void ViewBooks()
        {
            try
            {
                var books = _bookService.GetAllBooks();
                if (books == null || books.Count == 0)
                {
                    Console.WriteLine("No books found.");
                    return;
                }

                Console.WriteLine("Books:");
                foreach (var b in books)
                {
                    Console.WriteLine($"ID: {b.BookId} | Title: {b.Title} | Author: {b.Author} | Year: {b.PublicationYear} | Publisher: {b.Publisher} | ISBN: {b.ISBN} | CategoryId: {b.CategoryId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
            }
        }

        private void UpdateBook()
        {
            Console.WriteLine("Enter ISBN of the book to update:");
            var isbn = Console.ReadLine();

            Console.WriteLine("Enter new title:");
            var newTitle = Console.ReadLine();

            Console.WriteLine("Enter new author:");
            var newAuthor = Console.ReadLine();

            Console.WriteLine("Enter new publication year (numbers only):");
            var yearInput = Console.ReadLine();
            int newPublicationYear = 0;
            if (!int.TryParse(yearInput, out newPublicationYear))
            {
                Console.WriteLine("Invalid year entered, defaulting to 0.");
            }

            Console.WriteLine("Enter new publisher:");
            var newPublisher = Console.ReadLine();

            Console.WriteLine("Enter new category name:");
            var newCategoryName = Console.ReadLine();

            try
            {
                _bookService.UpdateBook(isbn ?? string.Empty, newTitle ?? string.Empty, newAuthor ?? string.Empty, newPublicationYear, newPublisher ?? string.Empty, newCategoryName ?? string.Empty);
                Console.WriteLine("Book updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
            }
        }

        private void DeleteBook()
        {
            Console.WriteLine("Enter ISBN of the book to delete:");
            var isbn = Console.ReadLine();

            Console.WriteLine("Are you sure you want to delete this book? (y/N)");
            var confirm = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(confirm) || !confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Delete cancelled.");
                return;
            }

            try
            {
                _bookService.RemoveBook(isbn ?? string.Empty);
                Console.WriteLine("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
            }
        }
         

    }
}