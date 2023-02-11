using System.Reflection;

namespace nagiashraf.CoursesApp.Services.Catalog.Data;

public class CourseContext : DbContext
{
    public CourseContext(DbContextOptions<CourseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<Picture> Pictures => Set<Picture>();
    public DbSet<CoursePreviewVideo> CoursePreviewVideos => Set<CoursePreviewVideo>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lecture> Lectures => Set<Lecture>();
    public DbSet<DownloadableFile> DownloadableFiles => Set<DownloadableFile>();
    public DbSet<LectureVideo> LectureVideos => Set<LectureVideo>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Subtitle> Subtitles => Set<Subtitle>();
    public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
}
