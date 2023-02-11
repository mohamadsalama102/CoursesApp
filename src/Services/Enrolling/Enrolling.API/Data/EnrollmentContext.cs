using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;
using System.Reflection;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Data;

public class EnrollmentContext : DbContext
{
    public EnrollmentContext(DbContextOptions<EnrollmentContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
}
