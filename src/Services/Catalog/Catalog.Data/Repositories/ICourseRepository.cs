using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;
using nagiashraf.CoursesApp.Services.Catalog.Core;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;

public interface ICourseRepository
{
    Task<PagedList<Course>> SearchAsync(CourseSearchModel courseSearchModel);
    Task<List<Course>> GetCoursesUnderReviewAsync();
    Task<Course?> GetCourseByIdAsync(int id, bool includePictureAndVideo = false, bool includeCurriculum = false);
    Task<Course?> GetCourseDetailsAsync(int id);
    Task<Picture?> GetCoursePictureAsync(int courseId);
    Task<CoursePreviewVideo?> GetCoursePreviewVideoAsync(int courseId);
    Task<decimal?> GetCoursePriceAsync(int courseId);
    Task<List<Section>?> GetCourseCurriculum(Course course);
    Task<TEntity> CreateDraftAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task SetIntendedLearnersAsync(Course course, List<string>? whatYouWillLearn, List<string>? requirements, 
        List<string>? whoIsThisCourseFor);
    Task SetCourseLandingPageAsync(Course course, string? title, string? subTitle, string? description, Language? language,
        Level? level, string? topic);
    Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : BaseEntity;

    // The CreateDraftAsync() generic method can't be used as the course total hours has to be incremented
    Task<LectureVideo> CreateLectureVideoDraftAndIncrementCourseHoursAsync(LectureVideo video);
    Task<LectureVideo?> GetLectureVideoByIdAsync(int id);
    Task SetPriceAsync(Course course, decimal price);
    Task<TEntity> UpdateDraftAsync<TEntity>(TEntity entityToUpdate) where TEntity : BaseEntity;
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task DeleteLectureVideoAndDecrementCourseHoursAsync(LectureVideo video);
    Task<bool> UserExitsInCatalogServiceAsync(string userId);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
    Task SubmitForAdminReviewAsync(Course course, Dictionary<int, int> sectionsOrders, Dictionary<int, int> lecturesOrders,
        Dictionary<int, int> questionsOrders);
    Task PublishCourseAsync(Course course);
    Task RejectCourseAsync(Course course);
    Task<Rating?> RateCourseAsync(Rating rating);
    Task<Rating?> GetRatingByIdAsync(int id);
    Task UpdateRatingAsync(Rating ratingToUpdate);
    Task DeleteRatingAsync(Rating rating);
    Task EnrollStudentInCourseAsync(string studentId, int courseId);
}
