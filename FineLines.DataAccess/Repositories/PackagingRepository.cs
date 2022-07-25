using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories
{
    internal class PackagingRepository : Repository<Packaging>, IPackagingRepository
    {
        private readonly ApplicationDbContext _db;
        public PackagingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            this.dbSet = _db.Set<Packaging>();
        }

        public void Update(Packaging obj)
        {
            dbSet.Update(obj);
        }
    }
}
