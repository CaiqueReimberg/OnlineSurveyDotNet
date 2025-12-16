using Infnet.OnlineSurveys.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data
{
    public class OnlineSurveysDbContext : DbContext
    {
        public OnlineSurveysDbContext(DbContextOptions<OnlineSurveysDbContext> options)
            : base(options)
        {
        }

        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Option> Options => Set<Option>();
        public DbSet<Response> Responses => Set<Response>();
        public DbSet<Answer> Answers => Set<Answer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Survey Configuration
            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Title).IsRequired();
                entity.HasMany(s => s.Questions)
                    .WithOne()
                    .HasForeignKey("SurveyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Question Configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Text).IsRequired();
                entity.Property<Guid>("SurveyId");
                entity.HasMany(q => q.Options)
                    .WithOne()
                    .HasForeignKey("QuestionId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Option Configuration
            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Text).IsRequired();
                entity.Property<Guid>("QuestionId");
            });

            // Response Configuration
            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired();
                entity.Property(r => r.Email).IsRequired();
                entity.HasMany(r => r.Answers)
                    .WithOne()
                    .HasForeignKey("ResponseId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Answer Configuration
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property<Guid>("ResponseId");
            });
        }
    }
}
