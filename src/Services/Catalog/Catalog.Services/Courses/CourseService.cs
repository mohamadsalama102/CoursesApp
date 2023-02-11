using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;

namespace nagiashraf.CoursesApp.Services.Catalog.Services.Courses;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CourseService(ICourseRepository courseRepository, ICloudinaryService cloudinaryService, IWebHostEnvironment webHostEnvironment)
    {
        _courseRepository = courseRepository;
        _cloudinaryService = cloudinaryService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> SetCoursePictureAsync(IFormFile? uploadedPicture, int courseId)
    {
        if (uploadedPicture == null)
            return null;

        var coursePicture = await _courseRepository.GetCoursePictureAsync(courseId);
        if (coursePicture != null && coursePicture.PublicId != null)
        {
            var deletionResult = await _cloudinaryService.DeleteFileAsync(coursePicture.PublicId);
            if (deletionResult.Error != null)
                return deletionResult.Error.Message;

            await _courseRepository.DeleteAsync(coursePicture);
        }

        var pictureUploadResult = await _cloudinaryService.AddPhotoAsync(uploadedPicture);
        if (pictureUploadResult.Error != null)
        {
            return pictureUploadResult.Error.Message;
        }

        await _courseRepository.CreateDraftAsync(new Picture
        {
            CourseId = courseId,
            PublicId = pictureUploadResult.PublicId,
            Url = pictureUploadResult.SecureUrl.AbsoluteUri
        });

        return null;
    }

    public async Task<string?> SetCoursePreviewVideoAsync(IFormFile? uploadedVideo, int courseId)
    {
        if (uploadedVideo == null)
            return null;

        var coursePreviewVideo = await _courseRepository.GetCoursePreviewVideoAsync(courseId);
        if (coursePreviewVideo != null && coursePreviewVideo.PublicId != null)
        {
            var deletionResult = await _cloudinaryService.DeleteFileAsync(coursePreviewVideo.PublicId);
            if (deletionResult.Error != null)
                return deletionResult.Error.Message;

            await _courseRepository.DeleteAsync(coursePreviewVideo);
        }

        var videoUploadResult = await _cloudinaryService.AddVideoAsync(uploadedVideo);
        if (videoUploadResult.Error != null)
            return videoUploadResult.Error.Message;

        await _courseRepository.CreateDraftAsync(new CoursePreviewVideo
        {
            CourseId = courseId,
            PublicId = videoUploadResult.PublicId,
            Url = videoUploadResult.SecureUrl.AbsoluteUri,
            Duration = videoUploadResult.Duration / (60 * 60)
        });

        return null;
    }

    public async Task<string?> SetLectureVideoAsync(IFormFile? uploadedVideo, int lectureId)
    {
        if (uploadedVideo == null)
            return "No video uploaded";

        var video = await _courseRepository.GetLectureVideoByIdAsync(lectureId);
        if (video != null && video.PublicId != null)
        {
            var deletionResult = await _cloudinaryService.DeleteFileAsync(video.PublicId);
            if (deletionResult.Error != null)
                return deletionResult.Error.Message;

            await _courseRepository.DeleteLectureVideoAndDecrementCourseHoursAsync(video);
        }

        var videoUploadResult = await _cloudinaryService.AddVideoAsync(uploadedVideo);
        if (videoUploadResult.Error != null)
            return videoUploadResult.Error.Message;

        var lectureVideo = new LectureVideo
        {
            PublicId = videoUploadResult.PublicId,
            Url = videoUploadResult.SecureUrl.AbsoluteUri,
            Duration = videoUploadResult.Duration / (60 * 60),
            LectureId = lectureId
        };

        await _courseRepository.CreateLectureVideoDraftAndIncrementCourseHoursAsync(lectureVideo);

        return null;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        var fileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
        var fileName = Guid.NewGuid().ToString() + fileExtension;
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        var fileUrlPath = Path.Combine(folderName, fileName);

        return fileUrlPath;
    }

    public void DeleteFile(string filePath)
    {
        string pathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
        File.Delete(pathToDelete);
    }
}
