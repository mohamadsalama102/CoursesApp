using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace nagiashraf.CoursesApp.Services.Catalog.Data;

public class CourseContextSeed
{
    public async static Task SeedAsync(CourseContext context, IWebHostEnvironment env)
    {
        var contentRootPath = env.ContentRootPath;

        await SeedAsync<Category>(context, contentRootPath, "Categories.json");
        await SeedAsync<SubCategory>(context, contentRootPath, "Subcategories.json");
        await SeedAsync<ApplicationUser>(context, contentRootPath, "ApplicationUsers.json");
        await SeedAsync<Course>(context, contentRootPath, "Courses.json");
        await SeedAsync<Section>(context, contentRootPath, "Sections.json");
        await SeedAsync<Lecture>(context, contentRootPath, "Lectures.json");
        await SeedAsync<LectureVideo>(context, contentRootPath, "LectureVideos.json");
        await SeedAsync<DownloadableFile>(context, contentRootPath, "DownloadableFiles.json");
        await SeedAsync<Quiz>(context, contentRootPath, "Quizzes.json");
        await SeedAsync<Question>(context, contentRootPath, "Questions.json");
        await SeedAsync<StudentCourse>(context, contentRootPath, "StudentCourses.json");
        await SeedAsync<Rating>(context, contentRootPath, "Ratings.json");
        await SeedAsync<CoursePreviewVideo>(context, contentRootPath, "CoursePreviewVideos.json");
        await SeedAsync<Picture>(context, contentRootPath, "CoursePictures.json");
    }

    private async static Task SeedAsync<T>(CourseContext context, string contentRootPath, string fileName) where T : class
    {
        if (context.Set<T>().Any())
            return;

        List<T>? objects = await GetDeserializedbjectsAsync<T>(fileName, contentRootPath);
        if (objects != null)
        {
            context.Set<T>().AddRange(objects);
        }

        await context.SaveChangesAsync();
    }

    private async static Task<List<T>?> GetDeserializedbjectsAsync<T>(string fileName, string contentRootPath)
    {
        string jsonFilePath = Path.Combine(contentRootPath, "SeedData", fileName);
        string jsonObjects = await File.ReadAllTextAsync(jsonFilePath);

        var deserializationOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<List<T>?>(jsonObjects, deserializationOptions);
    }
}
