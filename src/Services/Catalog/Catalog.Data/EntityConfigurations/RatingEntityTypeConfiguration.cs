using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class RatingEntityTypeConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasOne(r => r.Course)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CourseId);

        builder.HasOne(r => r.Student)
            .WithMany(s => s.Ratings)
            .HasForeignKey(r => r.StudentId);
    }
}
