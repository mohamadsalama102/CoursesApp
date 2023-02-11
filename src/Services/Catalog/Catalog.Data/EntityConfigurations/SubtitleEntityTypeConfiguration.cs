using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class SubtitleEntityTypeConfiguration : IEntityTypeConfiguration<Subtitle>
{
    public void Configure(EntityTypeBuilder<Subtitle> builder)
    {
        builder.Property(c => c.Language)
            .HasConversion<string>();

        builder.HasOne(s => s.Video)
                .WithMany(v => v.Subtitles)
                .HasForeignKey(a => a.VideoId);
    }
}
