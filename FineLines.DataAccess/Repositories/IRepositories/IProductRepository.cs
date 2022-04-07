using FineLines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

    }
}
