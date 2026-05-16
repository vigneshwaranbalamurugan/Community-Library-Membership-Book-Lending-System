using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.Ballibrary.Interfaces
{
    public interface IBookCategoryService
    {
        BookCategory AddCategory(string name, string? description = null);
        BookCategory UpdateCategory(Guid categoryId, string newName, string? newDescription = null);
        void RemoveCategory(Guid categoryId);
        BookCategory GetCategoryById(Guid categoryId);
        List<BookCategory> GetAllCategories();
        Guid GetCategoryIdByName(string name);
    }
}