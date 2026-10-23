using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public class TaskTrackerDbContext : DbContext
    {
        public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                value => value,
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

            modelBuilder.Entity<Project>(project =>
            {
                project.Property(p => p.Name).HasMaxLength(100);
                project.Property(p => p.Description).HasMaxLength(500);
                project.Property(p => p.CreatedAt).HasConversion(utcConverter);

                project.HasMany(p => p.Tasks)
                    .WithOne()
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                project.HasData(SeedData.Projects());
            });

            modelBuilder.Entity<TaskItem>(task =>
            {
                task.Property(t => t.Title).HasMaxLength(200);
                task.Property(t => t.Description).HasMaxLength(500);
                task.Property(t => t.CreatedAt).HasConversion(utcConverter);

                task.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

                task.HasData(SeedData.Tasks());
            });
        }
    }
}
