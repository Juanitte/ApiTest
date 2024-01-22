using ApiTest.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>().ToTable("ticket");
            modelBuilder.Entity<User>().ToTable("user");

            // Configuración adicional para la relación entre Ticket y User
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User);

            // Configuración adicional para el tipo de clave primaria de IdentityRole
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles").HasKey(r => r.Id);

        }
    }
}
