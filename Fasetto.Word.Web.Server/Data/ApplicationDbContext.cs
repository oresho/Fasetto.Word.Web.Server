using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Fasetto.Word.Web.Server.Data
{
    //The database representational model for our application
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        #region Public Properties
        public DbSet<SettingsDataModel> Settings { get; set; }
        #endregion

        // PASSING IN DBCONTEXTOPTIONS AS AN ARGUMENT FOR DB CREATION SO YOU CAN USE DI IN THE STARTUP
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)//where you define relationships between tables public key etc
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API
            modelBuilder.Entity<SettingsDataModel>().HasIndex(a => a.Name);
        }
    }

}
