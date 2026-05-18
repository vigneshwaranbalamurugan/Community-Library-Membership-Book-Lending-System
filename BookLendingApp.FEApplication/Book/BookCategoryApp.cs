using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;

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

					var choiceNumber = ConsoleInputValidator.ReadInt("Select an option:", 1, 5);

				switch (choiceNumber)
				{
					case 1: AddCategory(); break;
					case 2: ViewCategories(); break;
					case 3: UpdateCategory(); break;
					case 4: DeleteCategory(); break;
					case 5: return;
					default: Console.WriteLine("Invalid choice."); break;
				}
			}
		}

		private void AddCategory()
		{
			var name = ConsoleInputValidator.ReadRequiredString("Enter category name:");
			var desc = ConsoleInputValidator.ReadOptionalString("Enter description (optional):");

				try
				{
					_bookCategoryService.AddCategory(name, desc);
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
			var id = PromptCategorySelection();

			var name = ConsoleInputValidator.ReadRequiredString("Enter new name:");
			var desc = ConsoleInputValidator.ReadOptionalString("Enter new description (optional):");

				try
				{
					_bookCategoryService.UpdateCategory(id, name, desc);
					Console.WriteLine("Category updated.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error updating category: {ex.Message}");
				}
			}

		private void DeleteCategory()
		{
			var id = PromptCategorySelection();

			if (!ConsoleInputValidator.ReadYesNo("Are you sure?", defaultValue: false))
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

		private Guid PromptCategorySelection()
		{
			var cats = _bookCategoryService.GetAllCategories();
			if (cats == null || cats.Count == 0)
			{
				Console.WriteLine("No categories found.");
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
