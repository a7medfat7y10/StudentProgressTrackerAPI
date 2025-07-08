using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentProgressTracker.Models;

namespace StudentProgressTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<StudentProgress> StudentProgress { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Grade).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // StudentProgress Configuration
            modelBuilder.Entity<StudentProgress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CompletionPercentage).HasPrecision(5, 2);
                entity.Property(e => e.AssignmentCompletionRate).HasPrecision(5, 2);
                entity.Property(e => e.AssessmentScore).HasPrecision(5, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Progress)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.StudentId, e.Subject })
                    .IsUnique();
            });

            // Add indexes for better performance
            modelBuilder.Entity<Student>()
                .HasIndex(e => e.Grade);

            modelBuilder.Entity<StudentProgress>()
                .HasIndex(e => e.Subject);

            modelBuilder.Entity<StudentProgress>()
                .HasIndex(e => e.LastActivity);

            modelBuilder.Entity<StudentProgress>()
                .HasIndex(e => e.CreatedAt);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is Student || x.Entity is StudentProgress)
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    if (entity.Entity is Student student)
                    {
                        student.CreatedAt = DateTime.UtcNow;
                        student.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity.Entity is StudentProgress progress)
                    {
                        progress.CreatedAt = DateTime.UtcNow;
                        progress.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else if (entity.State == EntityState.Modified)
                {
                    if (entity.Entity is Student student)
                    {
                        student.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity.Entity is StudentProgress progress)
                    {
                        progress.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
