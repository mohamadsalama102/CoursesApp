using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class DownloadableFileEntityTypeConfiguration : IEntityTypeConfiguration<DownloadableFile>
{
    public void Configure(EntityTypeBuilder<DownloadableFile> builder)
    {
        builder.HasOne(f => f.Lecture)
               .WithMany(l => l.DownloadableFiles)
               .HasForeignKey(f => f.LectureId);
    }
}
