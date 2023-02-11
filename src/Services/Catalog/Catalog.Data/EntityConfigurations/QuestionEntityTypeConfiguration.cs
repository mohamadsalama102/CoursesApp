using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(q => new { q.Order, q.QuizId })
            .IsUnique();

        builder.HasOne(qn => qn.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(qn => qn.QuizId);

        builder.HasOne(q => q.RelatedLecture)
                .WithMany()
                .HasForeignKey(a => a.RelatedLectureId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(q => q.Answers, a =>
        {
            a.HasKey(k => new { k.Order, k.QuestionId });

            a.Property(a => a.Order)
                .ValueGeneratedNever();

            a.ToTable("Answers");

            a.Property(a => a.Content)
                .HasMaxLength(600);

            a.Property(a => a.Explanation)
                .HasMaxLength(600);
        });
    }
}
