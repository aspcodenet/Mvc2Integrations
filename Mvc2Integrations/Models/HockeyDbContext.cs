using Microsoft.EntityFrameworkCore;

namespace Mvc2Integrations.Models
{
    public class HockeyDbContext : DbContext
    {
        public HockeyDbContext(DbContextOptions<HockeyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<EmailSubscriber> EmailSubscribers { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Team { get; set; }
    }
}