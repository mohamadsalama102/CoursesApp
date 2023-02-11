using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.EntityConfigurations;

public class SubCategoryEntityTypeConfiguration : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(100);

        builder.HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId);
    }
}
