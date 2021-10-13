using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Linq;

namespace Rocky_DataAccess.Initializer
{
    /// <summary>
    /// This is the first Object we will run when first run the Application
    /// The application will do the following thiings:
    /// a. immigrate data model to DB
    /// b, create Admin and Customer roles
    /// c, create a new user and assign it a Admin role, who will be the first user
    /// We also need inject DbInitializer to the services
    /// We also need to add it to pipiline !
    /// </summary>
    
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext db,
                            UserManager<IdentityUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                             ILogger<DbInitializer> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public void Initalize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception e)
            {

            }
            //  create Roles :   Admin and customer
            //  create rolse and users mean to write data to Database

            if (!_roleManager.RoleExistsAsync(WC.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WC.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WC.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            //  create an User ( Who will be the first Admin)
            //  Here, if the password is too simple, then the user will not be created
            //  and we cannot get any notification about the error
            //  Try to study how to customize the Password policies !!!

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin Tester",
                PhoneNumber = "111-111-1111"
            }, "Admin123*").GetAwaiter().GetResult();

            //  assign the Admin role to the new-created user
            ApplicationUser user = _db.ApplicationUser.FirstOrDefault(u => u.Email == "admin@gmail.com");

            if (user != null)
            {             
                _userManager.AddToRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult();
            }
            else
            {
                _logger.LogWarning("DbInitializer-- failed to create the first user-- check LOG!");
            }



        }
    }
}
