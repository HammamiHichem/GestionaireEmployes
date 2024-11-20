#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using GestionaireEmployes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GestionaireEmployes.Models;


namespace GestionaireEmployes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }


       
       
    }
}