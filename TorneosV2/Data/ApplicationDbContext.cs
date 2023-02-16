using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TorneosV2.Model.Clases;

namespace TorneosV2.Data
{
    public class ApplicationDbContext : IdentityDbContext<Z110_Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Z100_Org> Organizaciones { get; set; }
        public DbSet<Z110_Usuario> Usuarios { get; set; }
        public DbSet<Z180_File> Archivos { get; set; }
        public DbSet<Z190_Bitacora> Bitacora { get; set; }
        //public DbSet<>  { get; set; }
    }
}