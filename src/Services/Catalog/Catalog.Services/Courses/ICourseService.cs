using Microsoft.AspNetCore.Http;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.Services.Courses;

public interface ICourseService
{
    Task<string?> SetCoursePictureAsync(IFormFile? uploadedPicture, int courseId);
    Task<string?> SetCoursePreviewVideoAsync(IFormFile? uploadedVideo, int courseId);
    Task<string?> SetLectureVideoAsync(IFormFile? uploadedVideo, int lectureId);
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    void DeleteFile(string filePath);
}
