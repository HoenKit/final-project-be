using final_project_be_Infrastructure.DAO;
using final_project_be_Application.Interface;
using System.Linq.Expressions;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Application.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IGenericDAO<T> _dao;

        public Repository(IGenericDAO<T> dao)
        {
            _dao = dao;
        }

        public IEnumerable<T> GetAll() => _dao.GetAll();

        public async Task<T?> GetByIdAsync(object id) => await _dao.GetByIdAsync(id);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => (IEnumerable<T>)_dao.FindAsync(predicate);

        public async void AddAsync(T entity) => await _dao.AddAsync(entity);

        public async void UpdateAsync(T entity) => await _dao.UpdateAsync(entity);

        public async void DeleteAsync(object id) => await _dao.DeleteAsync(id);
    }

}
