
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Data
{
    //DbContext is the final root class
    //IdentityDbContext has several tables in its hieritage chain
    //If we want to use Identity Functionality, we must inhirit IdentityDbContext


    //IdentityDbContext class has those default Identity-related DbSets
    public class ApplicationDbContext:/*DbContext*/IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Category>Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<ApplicationType> ApplicationType { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }    // ApplicationUser -> IdentityUser 
    }
}
