using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using System.Reflection.Emit;

namespace nagiashraf.CoursesApp.Services.Identity.API.Data.EntityConfigurations;

public class ApplicationUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasKey(k => new { k.UserId, k.RoleId });

        builder.HasOne(userRole => userRole.User)
            .WithMany(user => user.UserRoles)
            .HasForeignKey(userRole => userRole.UserId);

        builder.HasOne(userRole => userRole.Role)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(userRole => userRole.RoleId);

    }
}
