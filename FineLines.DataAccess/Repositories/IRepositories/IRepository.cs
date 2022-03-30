using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories.IRepositories
{
    //Generic Repository Interface
    public interface IRepository <T> where T : class
    {
        T GetFirstOrDefault(Expression<Func< T, bool>> filter);

        //get method
        T Get(int id);
        //get all
        IEnumerable <T> GetAll();
        void Add(T entity);
        void Remove(T entity);
//        void Remove(int entity);
        void RemoveRange(IEnumerable<T> entity);


    }
}
