using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories
{
    internal class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            this.dbSet = _db.Set<CoverType>();
        }

        public void Update(CoverType obj)
        {
            dbSet.Update(obj);
        }
    }
}
