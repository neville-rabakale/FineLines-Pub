using FineLinesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FineLinesApp.Data
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
