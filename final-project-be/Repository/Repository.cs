using final_project_be.DAO;
using final_project_be.Interface;
using System.Linq.Expressions;

namespace final_project_be.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly GenericDAO<T> _dao;

        public Repository(GenericDAO<T> dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<T?> GetByIdAsync(object id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dao.FindAsync(predicate);

        public async Task AddAsync(T entity) => await _dao.AddAsync(entity);

        public async Task UpdateAsync(T entity) => await _dao.UpdateAsync(entity);

        public async Task DeleteAsync(object id) => await _dao.DeleteAsync(id);
    }
}
