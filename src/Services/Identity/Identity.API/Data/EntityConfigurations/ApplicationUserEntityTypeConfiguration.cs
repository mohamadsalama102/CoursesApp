using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;

namespace nagiashraf.CoursesApp.Services.Identity.API.Data.EntityConfigurations;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Headline)
            .HasMaxLength(60);

        builder.OwnsOne(u => u.RefreshToken);

        builder.HasOne(u => u.Photo)
            .WithOne(ph => ph.User)
            .HasForeignKey<Photo>(ph => ph.UserId);
    }
}
