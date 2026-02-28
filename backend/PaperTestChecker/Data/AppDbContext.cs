using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Models;

namespace PaperTestChecker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<QuestionResult> QuestionResults { get; set; }
    public DbSet<GeneratedTest> GeneratedTests { get; set; }
    public DbSet<GeneratedTestItem> GeneratedTestItems { get; set; }
    public DbSet<TestAttempt> TestAttempts { get; set; }
    public DbSet<TestAttemptAnswer> TestAttemptAnswers { get; set; }

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

        modelBuilder.Entity<GeneratedTest>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.CreatedByUser)
             .WithMany()
             .HasForeignKey(t => t.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ForStudentUser)
             .WithMany()
             .HasForeignKey(t => t.ForStudentUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GeneratedTestItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.HasOne(i => i.GeneratedTest)
             .WithMany(t => t.Items)
             .HasForeignKey(i => i.GeneratedTestId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TestAttempt>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.GeneratedTest)
             .WithMany()
             .HasForeignKey(a => a.GeneratedTestId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Student)
             .WithMany()
             .HasForeignKey(a => a.StudentUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TestAttemptAnswer>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.TestAttempt)
             .WithMany(t => t.Answers)
             .HasForeignKey(a => a.TestAttemptId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
