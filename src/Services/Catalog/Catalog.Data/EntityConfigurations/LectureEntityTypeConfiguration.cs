using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class LectureEntityTypeConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.HasIndex(l => new { l.Order, l.SectionId })
            .IsUnique();

        builder.Property(l => l.Title)
            .HasMaxLength(80);

        builder.HasOne(l => l.Section)
                .WithMany(s => s.Lectures)
                .HasForeignKey(a => a.SectionId);

        builder.HasOne(l => l.Video)
            .WithOne(v => v.Lecture)
            .HasForeignKey<LectureVideo>(c => c.LectureId);
    }
}
