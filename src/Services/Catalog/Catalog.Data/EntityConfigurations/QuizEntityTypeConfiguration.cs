using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class QuizEntityTypeConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.Property(q => q.Title)
            .HasMaxLength(80);

        builder.HasOne(q => q.Section)
                .WithMany(s => s.Quizzes)
                .HasForeignKey(q => q.SectionId);
    }
}
