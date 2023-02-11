using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.Tests.Data;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=CoursesAppEnrollingDB_Tests;Trusted_Connection=True";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(
                        new Enrollment { StudentId = "user ID 1", CourseId = 1, CoursePrice = 59, PaymentSucceeded = true,
                            PaymentIntentId = "inent ID 1", ClientSecret = "client secret 1" },
                        new Enrollment { StudentId = "user ID 2", CourseId = 2, CoursePrice = 69, PaymentSucceeded = false,
                            PaymentIntentId = "inent ID 2", ClientSecret = "client secret 2"
                        });
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public EnrollmentContext CreateContext()
        => new EnrollmentContext(
            new DbContextOptionsBuilder<EnrollmentContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
