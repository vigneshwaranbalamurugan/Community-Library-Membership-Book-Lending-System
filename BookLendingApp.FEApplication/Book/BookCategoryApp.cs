using System;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Application.Book
{
	public class BookCategoryApp
	{
		private readonly IBookCategoryService _bookCategoryService;

		public BookCategoryApp(IBookCategoryService bookCategoryService)
		{
			_bookCategoryService = bookCategoryService;
		}

		public void CategoryMenu()
		{
			while (true)
			{
				Console.WriteLine("Category Menu:");
				Console.WriteLine("1. Add Category");
				Console.WriteLine("2. View Categories");
				Console.WriteLine("3. Update Category");
				Console.WriteLine("4. Delete Category");
				Console.WriteLine("5. Back");

				var choice = Console.ReadLine();
				switch (choice)
				{
					case "1": AddCategory(); break;
					case "2": ViewCategories(); break;
					case "3": UpdateCategory(); break;
					case "4": DeleteCategory(); break;
					case "5": return;
					default: Console.WriteLine("Invalid choice."); break;
				}
			}
		}

		private void AddCategory()
		{
			Console.WriteLine("Enter category name:");
			var name = Console.ReadLine();
			Console.WriteLine("Enter description (optional):");
			var desc = Console.ReadLine();

				try
				{
					_bookCategoryService.AddCategory(name ?? string.Empty, desc);
					Console.WriteLine("Category added.");
				}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding category: {ex.Message}");
			}
		}

		private void ViewCategories()
		{
			try
			{
				var cats = _bookCategoryService.GetAllCategories();
				if (cats == null || cats.Count == 0)
				{
					Console.WriteLine("No categories found.");
					return;
				}
				foreach (var c in cats)
				{
					Console.WriteLine($"ID: {c.CategoryId} | Name: {c.Name} | Description: {c.Description}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error retrieving categories: {ex.Message}");
			}
		}

		private void UpdateCategory()
		{
			Console.WriteLine("Enter category ID to update:");
			var idInput = Console.ReadLine();
			if (!Guid.TryParse(idInput, out var id))
			{
				Console.WriteLine("Invalid ID format.");
				return;
			}

			Console.WriteLine("Enter new name:");
			var name = Console.ReadLine();
			Console.WriteLine("Enter new description (optional):");
			var desc = Console.ReadLine();

				try
				{
					_bookCategoryService.UpdateCategory(id, name ?? string.Empty, desc);
					Console.WriteLine("Category updated.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error updating category: {ex.Message}");
				}
			}

		private void DeleteCategory()
		{
			Console.WriteLine("Enter category ID to delete:");
			var idInput = Console.ReadLine();
			if (!Guid.TryParse(idInput, out var id))
			{
				Console.WriteLine("Invalid ID format.");
				return;
			}

			Console.WriteLine("Are you sure? (y/N)");
			var confirm = Console.ReadLine();
			if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("Cancelled.");
				return;
			}

			try
			{
				_bookCategoryService.RemoveCategory(id);
				Console.WriteLine("Category deleted.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting category: {ex.Message}");
			}
		}
	}
}
