using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Data.EntityConfigurations;

public class EnrollmentEntityTypeConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();

        builder.Property(e => e.CoursePrice)
            .HasPrecision(18, 2);
    }
}
