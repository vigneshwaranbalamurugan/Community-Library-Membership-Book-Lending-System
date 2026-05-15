using BookLendingApp.ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.DALLibrary.Contexts;
using BookLendingApp.DALLibrary.Interfaces;

namespace BookLendingApp.DALLibrary.Repositories
{
    public class BookCategoryRepository: AbstractRepository<Guid, BookCategory>,IBookCategoryRepository
    {
        public BookCategoryRepository(BookLendingAppContext _context):base(_context)
        {  
        }

        public override BookCategory Create(BookCategory item)
        {
            _context.Categories.Add(item);
            _context.SaveChanges();
            return item;
        }
        public override BookCategory Update(Guid key, BookCategory item)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId == key);
            if (existingCategory == null)
                throw new KeyNotFoundException($"BookCategory with ID {key} not found.");
            
            existingCategory.Name = item.Name;
            existingCategory.Description = item.Description;
            
            _context.SaveChanges();
            return existingCategory;
        }
        public override BookCategory? Get(Guid key)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == key);
        }
        public override List<BookCategory> GetAll()
        {
            return _context.Categories.ToList();
        }
        public override BookCategory Delete(Guid key)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == key);
            if (category == null)
                throw new KeyNotFoundException($"BookCategory with ID {key} not found.");
            
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return category;
        }

        public BookCategory GetCategoryByName(string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == name);
            if (category == null)
                throw new KeyNotFoundException($"Category with name '{name}' not found.");
            return category;
        }

        public Guid GetCategoryIdByName(string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == name);
            if (category == null)
                throw new KeyNotFoundException($"Category with name '{name}' not found.");
            return category.CategoryId;
        }
    }
}