using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsRaffles.Data.Models;

namespace SportsRaffles.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Prize> Prizes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Competition>()
                .HasMany(e => e.Tickets)
                .WithOne(e => e.Competition)
                .HasForeignKey(e => e.CompetitionId);

            builder.Entity<Competition>()
                .HasMany(e => e.Prizes)
                .WithOne(e => e.Competition)
                .HasForeignKey(e => e.CompetitionId);
        }
    }
}
