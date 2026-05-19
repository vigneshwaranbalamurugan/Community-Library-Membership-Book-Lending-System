using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

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
           while (true)
           {
               ConsoleUi.WriteTitle("Book Menu");
               ConsoleUi.WriteMenuOptions(new[] { "Add Book", "View All Books", "View Books By Category", "Search Books", "Update Book", "Delete Book", "Back" });

               var choiceNumber = ConsoleInputValidator.ReadInt("Select an option:", 1, 7);

               switch (choiceNumber)
               {
                   case 1: AddBook(); break;
                   case 2: ViewBooks(); break;
                   case 3: ViewBooksByCategory(); break;
                   case 4: SearchBooks(); break;
                   case 5: UpdateBook(); break;
                   case 6: DeleteBook(); break;
                   case 7: return;
                   default:
                       ConsoleUi.WriteError("Invalid choice. Please try again.");
                       ConsoleUi.Pause();
                       break;
               }
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
            ConsoleUi.WriteSuccess("Book added successfully.");
            ConsoleUi.Pause();
        }
        
        public void ViewBooks()
        {
            try
            {
                var books = _bookService.GetAllBooks();
                if (books == null || books.Count == 0)
                {
                    ConsoleUi.WriteInfo("No books found.");
                    ConsoleUi.Pause();
                    return;
                }
                DisplayBooks(books);
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error retrieving books: {ex.Message}");
                ConsoleUi.Pause();
            }
        }

        private void UpdateBook()
        {
            var isbn = ConsoleInputValidator.ReadRequiredString("Enter ISBN of the book to update:");

            var books = _bookService.GetAllBooks() ?? new System.Collections.Generic.List<BookLendingApp.ModelLibrary.Models.Book>();
            var existingBook = books.FirstOrDefault(book => book.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));
            if (existingBook == null)
            {
                ConsoleUi.WriteError("Book not found for the given ISBN.");
                ConsoleUi.Pause();
                return;
            }

            var currentCategory = _bookCategoryService.GetCategoryById(existingBook.CategoryId);
            var currentCategoryName = currentCategory?.Name ?? string.Empty;

            ConsoleUi.WriteInfo($"Current values: Title={existingBook.Title}, Author={existingBook.Author}, Year={existingBook.PublicationYear}, Publisher={existingBook.Publisher}, Category={currentCategoryName}");

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
                ConsoleUi.WriteSuccess("Book updated successfully.");
                ConsoleUi.Pause();
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error updating book: {ex.Message}");
                ConsoleUi.Pause();
            }
        }

        private void DeleteBook()
        {
            var isbn = ConsoleInputValidator.ReadRequiredString("Enter ISBN of the book to delete:");

            if (!ConsoleInputValidator.ReadYesNo("Are you sure you want to delete this book?", defaultValue: false))
            {
                ConsoleUi.WriteInfo("Delete cancelled.");
                ConsoleUi.Pause();
                return;
            }

            try
            {
                _bookService.RemoveBook(isbn);
                ConsoleUi.WriteSuccess("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Cannot delete book: {ex.Message}");
                ConsoleUi.WriteInfo("Note: Ensure no copies or active borrows are linked to this book.");
            }
            ConsoleUi.Pause();
        }

        private Guid PromptCategorySelection()
        {
            var cats = _bookCategoryService.GetAllCategories() ?? new System.Collections.Generic.List<BookCategory>();
            if (cats.Count == 0)
            {
                ConsoleUi.WriteInfo("No categories found. Create one first.");
                ConsoleUi.Pause();
                return Guid.Empty;
            }

            var selected = ConsoleInputValidator.PromptSelection(
                "Select a category:",
                cats,
                c => $"Name: {c.Name} | Description: {c.Description}");

            return selected.CategoryId;
        }

        public void ViewBooksByCategory()
        {
            var categoryId = PromptCategorySelection();
            if (categoryId == Guid.Empty) return;

            var allBooks = _bookService.GetAllBooks() ?? new List<BookLendingApp.ModelLibrary.Models.Book>();
            var filtered = allBooks.Where(b => b.CategoryId == categoryId).ToList();

            if (filtered.Count == 0)
            {
                ConsoleUi.WriteInfo("No books found in this category.");
                ConsoleUi.Pause();
                return;
            }

            DisplayBooks(filtered);
        }

        public void SearchBooks()
        {
            ConsoleUi.WriteTitle("Search Books");
            ConsoleUi.WriteMenuOptions(new[] { "Search by Title", "Search by Author", "Search by Year", "Cancel" });
            var choice = ConsoleInputValidator.ReadInt("Select search criteria:", 1, 4);
            if (choice == 4) return;

            var allBooks = _bookService.GetAllBooks() ?? new List<BookLendingApp.ModelLibrary.Models.Book>();
            List<BookLendingApp.ModelLibrary.Models.Book> results;

            switch (choice)
            {
                case 1:
                    var title = ConsoleInputValidator.ReadRequiredString("Enter title keyword:");
                    results = allBooks.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case 2:
                    var author = ConsoleInputValidator.ReadRequiredString("Enter author name:");
                    results = allBooks.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case 3:
                    var year = ConsoleInputValidator.ReadInt("Enter publication year:", 1, 9999);
                    results = allBooks.Where(b => b.PublicationYear == year).ToList();
                    break;
                default:
                    return;
            }

            if (results.Count == 0)
            {
                ConsoleUi.WriteInfo("No matching books found.");
                ConsoleUi.Pause();
                return;
            }

            DisplayBooks(results);
        }

        private void DisplayBooks(IEnumerable<BookLendingApp.ModelLibrary.Models.Book> books)
        {
            var rows = new List<string>();
            foreach (var b in books)
            {
                rows.Add($"Title: {b.Title} | Author: {b.Author} | Year: {b.PublicationYear} | Publisher: {b.Publisher} | ISBN: {b.ISBN} | CategoryId: {b.CategoryId}");
            }
            ConsoleUi.WriteTable(rows);
            ConsoleUi.Pause();
        }

    }
}