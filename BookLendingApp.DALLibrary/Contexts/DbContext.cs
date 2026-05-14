using Microsoft.EntityFrameworkCore;

namespace BookLendingApp.DALLibrary.Contexts
{
    public class BookLendingAppContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=booklendingappdb;Username=postgres;Password=978681");
        }
    }
}