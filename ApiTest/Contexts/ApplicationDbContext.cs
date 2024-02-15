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
            modelBuilder.Entity<Attachment>().ToTable("attachment");

            // Configuración adicional para la relación entre Ticket y User
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets) // Agrega la propiedad de navegación inversa en la clase User
                .HasForeignKey(t => t.UserID)
                .IsRequired(false); // Puedes ajustar esto según tus necesidades

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.Ticket)
                .HasForeignKey(m => m.TicketID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Attachment>()
                .HasOne(t => t.Message)
                .WithMany(m => m.AttachmentPaths)
                .HasForeignKey(t => t.MessageID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración adicional para el tipo de clave primaria de IdentityRole
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles").HasKey(r => r.Id);

        }
    }
}
