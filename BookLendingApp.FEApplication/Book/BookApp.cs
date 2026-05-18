using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Validation;

namespace BookLendingApp.Application.Book
{
    public class BookApp
    {
       private readonly IBookService _bookService;
       private readonly IBookCategoryService _bookCategoryService;

       public BookApp(IBookService bookService, IBookCategoryService bookCategoryService)
       {
           _bookService = bookService;
           _bookCategoryService = bookCategoryService;
       }

       public void BookMenu()
       {
           Console.WriteLine("Book Menu:");
           Console.WriteLine("1. Add Book");
           Console.WriteLine("2. View Books");
           Console.WriteLine("3. Update Book");
           Console.WriteLine("4. Delete Book");
           Console.WriteLine("5. Back to Main Menu");

           var choiceNumber = ConsoleInputValidator.ReadInt("Select an option:", 1, 5);

           switch (choiceNumber)
           {
               case 1:
                   AddBook();
                   break;
               case 2:
                   ViewBooks();
                   break;
               case 3:
                   UpdateBook();
                   break;
               case 4:
                   DeleteBook();
                   break;
               case 5:
                   return;
               default:
                   Console.WriteLine("Invalid choice. Please try again.");
                   break;
           }
       }

        private void AddBook()
        {
            var title = ConsoleInputValidator.ReadRequiredString("Enter book title:");

            var author = ConsoleInputValidator.ReadRequiredString("Enter book author:");

            var publicationYear = ConsoleInputValidator.ReadInt("Enter publication year:", 1, 9999);

            var publisher = ConsoleInputValidator.ReadRequiredString("Enter publisher:");

            var categoryId = PromptCategorySelection();
            if (categoryId == Guid.Empty) return;
            var categoryName = _bookCategoryService.GetCategoryById(categoryId)?.Name ?? ConsoleInputValidator.ReadRequiredString("Enter category name:");

            var isbn = ConsoleInputValidator.ReadRequiredString("Enter ISBN:");

            _bookService.AddBook(title, author, publicationYear, publisher, categoryName, isbn);
            Console.WriteLine("Book added successfully.");
        }
        
        public void ViewBooks()
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
                    Console.WriteLine($"Title: {b.Title} | Author: {b.Author} | Year: {b.PublicationYear} | Publisher: {b.Publisher} | ISBN: {b.ISBN} | CategoryId: {b.CategoryId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
            }
        }

        private void UpdateBook()
        {
            var isbn = ConsoleInputValidator.ReadRequiredString("Enter ISBN of the book to update:");

            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            var existingBook = books.FirstOrDefault(book => book.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));
            if (existingBook == null)
            {
                Console.WriteLine("Book not found for the given ISBN.");
                return;
            }

            var currentCategory = _bookCategoryService.GetCategoryById(existingBook.CategoryId);
            var currentCategoryName = currentCategory?.Name ?? string.Empty;

            Console.WriteLine($"Current values: Title={existingBook.Title}, Author={existingBook.Author}, Year={existingBook.PublicationYear}, Publisher={existingBook.Publisher}, Category={currentCategoryName}");

            var newTitle = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter new title", existingBook.Title);

            var newAuthor = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter new author", existingBook.Author);

            var newPublicationYear = ConsoleInputValidator.ReadIntWithDefault("Enter new publication year", existingBook.PublicationYear, 1, 9999);

            var newPublisher = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter new publisher", existingBook.Publisher);

            var newCategoryName = currentCategoryName;
            if (ConsoleInputValidator.ReadYesNo($"Change category? Current is '{currentCategoryName}'", defaultValue: false))
            {
                var newCategoryId = PromptCategorySelection();
                if (newCategoryId == Guid.Empty) return;
                newCategoryName = _bookCategoryService.GetCategoryById(newCategoryId)?.Name ?? currentCategoryName;
            }

            try
            {
                _bookService.UpdateBook(isbn, newTitle, newAuthor, newPublicationYear, newPublisher, newCategoryName);
                Console.WriteLine("Book updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
            }
        }

        private void DeleteBook()
        {
            var isbn = ConsoleInputValidator.ReadRequiredString("Enter ISBN of the book to delete:");

            if (!ConsoleInputValidator.ReadYesNo("Are you sure you want to delete this book?", defaultValue: false))
            {
                Console.WriteLine("Delete cancelled.");
                return;
            }

            try
            {
                _bookService.RemoveBook(isbn);
                Console.WriteLine("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
            }
        }

        private Guid PromptCategorySelection()
        {
            var cats = _bookCategoryService.GetAllCategories() ?? new System.Collections.Generic.List<BookCategory>();
            if (cats.Count == 0)
            {
                Console.WriteLine("No categories found. Create one first.");
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a category:",
                cats,
                c => $"{c.Name} | {c.Description}");

            return selected.CategoryId;
        }

    }
}