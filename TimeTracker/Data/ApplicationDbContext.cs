using Microsoft.EntityFrameworkCore;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        
        public DbSet<User> User { get; set; }

        public DbSet<Roles> Roles { get; set; }

        public DbSet<RefreshTokenProvider> RefreshTokenProvider { get; set; }
              
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.UserId).IsRequired().HasMaxLength(int.MaxValue);
                    entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                    entity.Property(p => p.Surname).IsRequired().HasMaxLength(255);
                    entity.Property(p => p.Email).IsRequired().HasMaxLength(255);
                    entity.Property(p => p.PasswordHash).IsRequired().HasColumnType("varbinary(max)");
                    entity.Property(p => p.PasswordSalt).IsRequired().HasColumnType("varbinary(max)");
                    entity.Property(p => p.StartedWorkDayAt).IsRequired().HasColumnType("datetime");
                    entity.Property(p => p.FinishedWorkDayAt).IsRequired().HasColumnType("datetime");
                    entity.Property(p => p.Break).IsRequired().HasMaxLength(59);
                });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<RefreshTokenProvider>(entity =>
            {
                entity.HasKey(p => p.Id);
            });

        }

        public async Task RemoveExpiredRefreshTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = await RefreshTokenProvider
                .Where(t => t.RefreshTokenExpiresAt <= now)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                RefreshTokenProvider.RemoveRange(expiredTokens);
                await SaveChangesAsync();
            }
        }
    }
}
