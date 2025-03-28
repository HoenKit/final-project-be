using final_project_be.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace final_project_be.DAO
{
    public class GenericDAO<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private IDbContextTransaction? _transaction;

        public GenericDAO(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public T? GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(object id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        // Transaction handling without async
        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }
    }
}
