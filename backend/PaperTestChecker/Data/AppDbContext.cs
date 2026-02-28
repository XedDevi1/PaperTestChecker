using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Models;

namespace PaperTestChecker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<QuestionResult> QuestionResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Submission>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.User)
             .WithMany()
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionResult>(e =>
        {
            e.HasKey(q => q.Id);
            e.HasOne(q => q.Submission)
             .WithMany(s => s.QuestionResults)
             .HasForeignKey(q => q.SubmissionId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
