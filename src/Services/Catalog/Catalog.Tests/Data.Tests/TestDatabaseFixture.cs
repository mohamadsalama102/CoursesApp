using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Data.Tests;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=CoursesAppDB_Tests;Trusted_Connection=True";

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
                    //context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    Seed(context);
                }

                _databaseInitialized = true;
            }
        }
    }

    public CourseContext CreateContext()
        => new CourseContext(
            new DbContextOptionsBuilder<CourseContext>()
                .UseSqlServer(ConnectionString)
                .Options);

    public void Seed(CourseContext context)
    {
        Seed<Category>(context, "Categories.json");
        Seed<SubCategory>(context, "Subcategories.json");
        Seed<ApplicationUser>(context, "ApplicationUsers.json");
        Seed<Course>(context, "Courses.json");
        Seed<Section>(context, "Sections.json");
        Seed<Lecture>(context, "Lectures.json");
        Seed<LectureVideo>(context, "LectureVideos.json");
        Seed<Quiz>(context, "Quizzes.json");
        Seed<Question>(context, "Questions.json");
        Seed<StudentCourse>(context, "StudentCourses.json");
        Seed<Rating>(context, "Ratings.json");
        Seed<CoursePreviewVideo>(context, "CoursePreviewVideos.json");
        Seed<Picture>(context, "CoursePictures.json");
    }

    private void Seed<T>(CourseContext context, string fileName) where T : class
    {
        if (context.Set<T>().Any())
            return;

        List<T>? objects = GetDeserializedbjects<T>(fileName);
        if (objects != null)
        {
            context.Set<T>().AddRange(objects);
        }

        context.SaveChanges();
    }

    private List<T>? GetDeserializedbjects<T>(string fileName)
    {
        string currentDirectory = Environment.CurrentDirectory;
        string jsonFilePath = Path.Combine(currentDirectory, "SeedData", fileName);
        string jsonObjects = File.ReadAllText(jsonFilePath);

        var deserializationOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<List<T>?>(jsonObjects, deserializationOptions);
    }
}
