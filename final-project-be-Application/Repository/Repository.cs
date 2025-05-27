using final_project_be_Infrastructure.DAO;
using final_project_be_Application.Interface;
using System.Linq.Expressions;

namespace final_project_be_Application.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly GenericDAO<T> _dao;

        public Repository(GenericDAO<T> dao)
        {
            _dao = dao;
        }

        public IEnumerable<T> GetAll() => _dao.GetAll();

        public T? GetById(object id) => _dao.GetById(id);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _dao.Find(predicate);

        public void Add(T entity) => _dao.Add(entity);

        public void Update(T entity) => _dao.Update(entity);

        public void Delete(object id) => _dao.Delete(id);
    }

}
