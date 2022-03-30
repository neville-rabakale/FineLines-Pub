using FineLines.DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            //for categories this does -- _db.Categories.Add(entity) -- Generic for all Model Types
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            //First we need to query the db
            IQueryable<T> query = dbSet;
            //then return it as a list
            return query.ToList();

        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            //First we need to query the db
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            //then return it as a list
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }


        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
