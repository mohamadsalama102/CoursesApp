using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;
using nagiashraf.CoursesApp.Services.Catalog.Services.Courses;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Services.Tests.Courses;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
    private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;

    public CourseServiceTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _cloudinaryServiceMock = new Mock<ICloudinaryService>();
        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
    }

    [Fact]
    public async Task CanSetCoursePicture()
    {
        //Arrange
        var fileName = "test.jpg";
        var stream = new MemoryStream();

        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        var courseId = 123;
        var picture = new Picture
        {
            PublicId = "Test Public ID",
            Url = "Test URL"
        };

        _courseRepositoryMock
            .Setup(r => r.GetCoursePictureAsync(courseId))
            .ReturnsAsync(picture);

        _cloudinaryServiceMock
            .Setup(c => c.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(new DeletionResult { });

        _cloudinaryServiceMock
            .Setup(c => c.AddPhotoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(new ImageUploadResult { PublicId = "Test Public ID", SecureUrl = new Uri("http://www.test.com") } );

        //Act
        var courseService = new CourseService(_courseRepositoryMock.Object, _cloudinaryServiceMock.Object, _webHostEnvironmentMock.Object);
        var errorMessage = await courseService.SetCoursePictureAsync(file, courseId);
        var nullFileErrorMessage = await courseService.SetCoursePictureAsync(null, courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetCoursePictureAsync(courseId));
        _cloudinaryServiceMock.Verify(c => c.DeleteFileAsync(It.IsAny<string>()));
        _cloudinaryServiceMock.Verify(c => c.AddPhotoAsync(It.IsAny<IFormFile>()));
        Assert.Null(errorMessage);
        Assert.Null(nullFileErrorMessage);
    }

    [Fact]
    public async Task CanSetCoursePreviewVideo()
    {
        //Arrange
        var fileName = "test.mp4";
        var stream = new MemoryStream();

        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        var courseId = 123;
        var video = new CoursePreviewVideo
        {
            PublicId = "Test Public ID",
            Url = "Test URL"
        };

        _courseRepositoryMock
            .Setup(r => r.GetCoursePreviewVideoAsync(courseId))
            .ReturnsAsync(video);

        _cloudinaryServiceMock
            .Setup(c => c.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(new DeletionResult { });

        _cloudinaryServiceMock
            .Setup(c => c.AddVideoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(new VideoUploadResult { PublicId = "Test Public ID", SecureUrl = new Uri("http://www.test.com") });

        //Act
        var courseService = new CourseService(_courseRepositoryMock.Object, _cloudinaryServiceMock.Object, _webHostEnvironmentMock.Object);
        var errorMessage = await courseService.SetCoursePreviewVideoAsync(file, courseId);
        var nullFileErrorMessage = await courseService.SetCoursePreviewVideoAsync(null, courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetCoursePreviewVideoAsync(courseId));
        _cloudinaryServiceMock.Verify(c => c.DeleteFileAsync(It.IsAny<string>()));
        _cloudinaryServiceMock.Verify(c => c.AddVideoAsync(It.IsAny<IFormFile>()));
        Assert.Null(errorMessage);
        Assert.Null(nullFileErrorMessage);
    }

    [Fact]
    public async Task CanSetLectureVideo()
    {
        //Arrange
        var fileName = "test.mp4";
        var stream = new MemoryStream();

        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        var lectureId = 123;
        var video = new LectureVideo
        {
            PublicId = "Test Public ID",
            Url = "Test URL"
        };

        _courseRepositoryMock
            .Setup(r => r.GetLectureVideoByIdAsync(lectureId))
            .ReturnsAsync(video);

        _cloudinaryServiceMock
            .Setup(c => c.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(new DeletionResult { });

        _cloudinaryServiceMock
            .Setup(c => c.AddVideoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(new VideoUploadResult { PublicId = "Test Public ID", SecureUrl = new Uri("http://www.test.com") });

        //Act
        var courseService = new CourseService(_courseRepositoryMock.Object, _cloudinaryServiceMock.Object, _webHostEnvironmentMock.Object);
        var errorMessage = await courseService.SetLectureVideoAsync(file, lectureId);
        var nullFileErrorMessage = await courseService.SetLectureVideoAsync(null, lectureId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetLectureVideoByIdAsync(lectureId));
        _cloudinaryServiceMock.Verify(c => c.DeleteFileAsync(It.IsAny<string>()));
        _cloudinaryServiceMock.Verify(c => c.AddVideoAsync(It.IsAny<IFormFile>()));
        Assert.Null(errorMessage);
        Assert.NotNull(nullFileErrorMessage);
    }
}
