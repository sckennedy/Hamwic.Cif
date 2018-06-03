using Hamwic.Cif.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hamwic.Cif.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Public Properties

        public DbSet<School> Schools { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor expecting database options to be provided
        /// </summary>
        /// <param name="options">The database context options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {   
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}