using System;
using System.Collections.Generic;
using BookLendingApp.ModelLibrary.Models;
using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.Ballibrary.Interfaces;

namespace BookLendingApp.Ballibrary.Services
{
	public class BookCategoryService: IBookCategoryService
	{
		private readonly IBookCategoryRepository _categoryRepository;

		public BookCategoryService(IBookCategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public BookCategory AddCategory(string name, string? description = null)
		{
			var category = new BookCategory { Name = name ?? string.Empty, Description = description };
			return _categoryRepository.Create(category);
		}

		public BookCategory UpdateCategory(Guid categoryId, string name, string? description = null)
		{
			var updated = new BookCategory { CategoryId = categoryId, Name = name ?? string.Empty, Description = description };
			return _categoryRepository.Update(categoryId, updated);
		}

		public void RemoveCategory(Guid categoryId)
		{
			_categoryRepository.Delete(categoryId);
		}

		public BookCategory GetCategoryById(Guid categoryId)
		{
			return _categoryRepository.Get(categoryId) ?? throw new KeyNotFoundException($"Category {categoryId} not found.");
		}

		public List<BookCategory> GetAllCategories()
		{
			return _categoryRepository.GetAll();
		}

		public Guid GetCategoryIdByName(string name)
		{
			return _categoryRepository.GetCategoryIdByName(name);
		}
	}
}

