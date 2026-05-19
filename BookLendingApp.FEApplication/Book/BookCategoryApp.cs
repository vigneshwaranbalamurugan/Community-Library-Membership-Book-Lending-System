using System;
using BookLendingApp.Ballibrary.Interfaces;
using BookLendingApp.FEApplication.Validation;
using BookLendingApp.FEApplication.Common;

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
				ConsoleUi.WriteTitle("Category Menu");
				ConsoleUi.WriteMenuOptions(new[] { "Add Category", "View Categories", "Update Category", "Delete Category", "Back" });

					var choiceNumber = ConsoleInputValidator.ReadInt("Select an option:", 1, 5);

				switch (choiceNumber)
				{
					case 1: AddCategory(); break;
					case 2: ViewCategories(); break;
					case 3: UpdateCategory(); break;
					case 4: DeleteCategory(); break;
					case 5: return;
					default: ConsoleUi.WriteError("Invalid choice."); ConsoleUi.Pause(); break;
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
					ConsoleUi.WriteSuccess("Category added.");
					ConsoleUi.Pause();
				}
			catch (Exception ex)
			{
					ConsoleUi.WriteError($"Error adding category: {ex.Message}");
					ConsoleUi.Pause();
			}
		}

		private void ViewCategories()
		{
			try
			{
				var cats = _bookCategoryService.GetAllCategories();
				if (cats == null || cats.Count == 0)
				{
					ConsoleUi.WriteInfo("No categories found.");
					ConsoleUi.Pause();
					return;
				}
				var rows = new System.Collections.Generic.List<string>();
				foreach (var c in cats)
				{
					rows.Add($"ID: {c.CategoryId} | Name: {c.Name} | Description: {c.Description}");
				}
				ConsoleUi.WriteTable(rows);
				ConsoleUi.Pause();
			}
			catch (Exception ex)
			{
				ConsoleUi.WriteError($"Error retrieving categories: {ex.Message}");
				ConsoleUi.Pause();
			}
		}

		private void UpdateCategory()
		{
			var id = PromptCategorySelection();
			if (id == Guid.Empty)
			{
				return;
			}

			var existing = _bookCategoryService.GetCategoryById(id);
			if (existing == null)
			{
				ConsoleUi.WriteError("Category not found.");
				ConsoleUi.Pause();
				return;
			}

			ConsoleUi.WriteInfo($"Current values: Name={existing.Name}, Description={existing.Description}");

			var name = ConsoleInputValidator.ReadRequiredStringWithDefault("Enter new name", existing.Name);
			var desc = ConsoleInputValidator.ReadOptionalStringWithDefault("Enter new description (optional)", existing.Description);

				try
				{
					_bookCategoryService.UpdateCategory(id, name, desc);
					ConsoleUi.WriteSuccess("Category updated.");
					ConsoleUi.Pause();
				}
				catch (Exception ex)
				{
					ConsoleUi.WriteError($"Error updating category: {ex.Message}");
					ConsoleUi.Pause();
				}
			}

		private void DeleteCategory()
		{
			var id = PromptCategorySelection();

			if (!ConsoleInputValidator.ReadYesNo("Are you sure?", defaultValue: false))
			{
				ConsoleUi.WriteInfo("Cancelled.");
				ConsoleUi.Pause();
				return;
			}

			try
			{
				_bookCategoryService.RemoveCategory(id);
				ConsoleUi.WriteSuccess("Category deleted.");
			}
			catch (Exception ex)
			{
				ConsoleUi.WriteError($"Cannot delete category: {ex.Message}");
				ConsoleUi.WriteInfo("Note: Ensure no books are currently assigned to this category.");
			}
			ConsoleUi.Pause();
		}

		private Guid PromptCategorySelection()
		{
			var cats = _bookCategoryService.GetAllCategories();
			if (cats == null || cats.Count == 0)
			{
				ConsoleUi.WriteInfo("No categories found.");
				ConsoleUi.Pause();
				return Guid.Empty;
			}

			var selected = ConsoleInputValidator.PromptSelection(
				"Select a category:",
				cats,
				c => $"Name: {c.Name} | Description: {c.Description}");

			return selected.CategoryId;
		}
	}
}
