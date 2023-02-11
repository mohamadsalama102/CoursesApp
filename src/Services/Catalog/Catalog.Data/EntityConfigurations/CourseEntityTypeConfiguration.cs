using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Title)
            .HasMaxLength(60);

        builder.Property(c => c.SubTitle)
            .HasMaxLength(120);

        builder.Property(c => c.Topic)
            .HasMaxLength(60);

        builder.Property(c => c.Price)
            .HasPrecision(18, 2);

        builder.Property(c => c.Level)
            .HasConversion<string>();

        builder.Property(c => c.Language)
            .HasConversion<string>();

        builder.HasOne(c => c.SubCategory)
            .WithMany(sc => sc.Courses)
            .HasForeignKey(c => c.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Picture)
            .WithOne(p => p.Course)
            .HasForeignKey<Picture>(c => c.CourseId);

        builder.HasOne(c => c.PreviewVideo)
            .WithOne(v => v.Course)
            .HasForeignKey<CoursePreviewVideo>(c => c.CourseId);

        builder.OwnsOne(a => a.Discount, d =>
        {
            d.Property(d => d.Amount).HasPrecision(18, 2);
        });

        builder.HasOne(c => c.Instructor)
                .WithMany(i => i.CreatedCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Students)
            .WithMany(s => s.EnrolledInCourses)
            .UsingEntity<StudentCourse>(
                joint => joint
                    .HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(sc => sc.StudentId),
                joint => joint
                    .HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(sc => sc.CourseId),
                joint => joint.HasKey(k => new { k.CourseId, k.StudentId }));
    }
}