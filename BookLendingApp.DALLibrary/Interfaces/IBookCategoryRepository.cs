using BookLendingApp.ModelLibrary.Models;

namespace BookLendingApp.DALLibrary.Interfaces
{
    public interface IBookCategoryRepository : IRepository<Guid, BookCategory>
    {
        BookCategory GetCategoryByName(string name);      
        Guid GetCategoryIdByName(string name);  
    }
}