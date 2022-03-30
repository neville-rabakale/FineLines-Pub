using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            this.dbSet = _db.Set<Category>();
        }


        //If something goes wrong with category update, start here
        //Use _db.Categories.Update(obj)
        public void Update(Category obj)
        {
            dbSet.Update(obj);
        }
    }
}
