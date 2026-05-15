using BookLendingApp.DALLibrary.Interfaces;
using BookLendingApp.DALLibrary.Contexts;

namespace BookLendingApp.DALLibrary.Repositories
{
    public abstract class AbstractRepository<K,T> : IRepository<K,T> where T : class
    {
        protected readonly BookLendingAppContext _context;

        public AbstractRepository(BookLendingAppContext context)
        {
            _context = context;
        }

        public abstract T Create(T item);
        public abstract T? Get(K key);
        public abstract List<T> GetAll();
        public abstract T Update(K key, T item);
        public abstract T Delete(K key);
    }
}