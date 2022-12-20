using Microsoft.EntityFrameworkCore;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                 .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                 .AddJsonFile("appsettings.json", optional: false)
                 .AddJsonFile($"appsettings.{envName}.json", optional: false)
                 .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<User> User { get; set; }

        public DbSet<Roles> Roles { get; set; }
              
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.UserId).IsRequired().HasMaxLength(int.MaxValue);
                    entity.Property(p => p.Name).IsRequired();
                    entity.Property(p => p.Surname).IsRequired();
                    entity.Property(p => p.Email).IsRequired();
                    entity.Property(p => p.StartedWorkDayAt).IsRequired().HasColumnType("datetime");
                    entity.Property(p => p.FinishedWorkDayAt).IsRequired().HasColumnType("datetime");
                    entity.Property(p => p.Break).IsRequired().HasMaxLength(59);
                });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasNoKey();
            });

        }
    }
}
