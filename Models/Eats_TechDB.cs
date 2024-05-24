using Microsoft.EntityFrameworkCore;

namespace Eats_Tech.Models
{
    public class Eats_TechDB : DbContext
    {
        public Eats_TechDB(DbContextOptions<Eats_TechDB> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Menu> Menu { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
    }
}
