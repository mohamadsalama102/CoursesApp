using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;

public interface ICloudinaryService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<VideoUploadResult> AddVideoAsync(IFormFile file);
    Task<DeletionResult> DeleteFileAsync(string publicId);
}
