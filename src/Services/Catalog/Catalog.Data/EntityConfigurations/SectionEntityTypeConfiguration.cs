using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class SectionEntityTypeConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasIndex(s => new { s.Order, s.CourseId })
            .IsUnique();

        builder.Property(s => s.Title)
            .HasMaxLength(80);

        builder.Property(s => s.LearningObjective)
            .HasMaxLength(200);

        builder.HasOne(s => s.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(s => s.CourseId);
    }
}
