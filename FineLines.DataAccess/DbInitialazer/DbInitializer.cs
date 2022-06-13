using FineLines.Models;
using FineLines.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.DataAccess.DbInitialazer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;


        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            // Apply migrations if not applied
            try
            {
                //check if there has been any migrations
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    //if there are pending migrations -> migrate
                    _db.Database.Migrate();
                }

            }
            catch (Exception ex)
            {

            }

            ////check if any role exists, if not create all roles.
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                //if roles not created, create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "neville.rabakale@gmail.com",
                    Email = "neville.rabakale@gmail.com",
                    Name = "Neville Rabakale",
                    City = "Stockholm",
                    StreetAddress = "123 Stockholm Street",
                    PhoneNumber = "0823527129",
                    County = "Stock",
                    PostalCode = "22102"

                }, "Admin123*").GetAwaiter().GetResult();

                //assign created user Role of User
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "neville.rabakale@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;

        }
    }
}
