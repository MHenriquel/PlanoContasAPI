using Microsoft.EntityFrameworkCore;

namespace PlanoContasAPI.Models
{
    public class PlanoContasContext : DbContext
    {
        public PlanoContasContext(DbContextOptions<PlanoContasContext> options)
            : base(options)
        {
        }

        public DbSet<PlanoContas> PlanoContas { get; set; }
    }
}