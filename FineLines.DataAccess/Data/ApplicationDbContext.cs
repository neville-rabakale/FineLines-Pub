using FineLines.Models;
using Microsoft.EntityFrameworkCore;

namespace FineLines.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //Create dataset -> DataTable for "Categories" of Category Model
        public DbSet<Category> Categories { get; set; }
    }
}
