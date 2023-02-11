using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using nagiashraf.CoursesApp.Services.Catalog.API.Controllers;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.Core;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;
using nagiashraf.CoursesApp.Services.Catalog.Services.Courses;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using System.Net;
using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Api.Tests.Controllers;

public class CourseControllerTest
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IIdentityGrpcClient> _identityGrpcClientMock;

    public CourseControllerTest()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _courseServiceMock = new Mock<ICourseService>();
        _cloudinaryServiceMock = new Mock<ICloudinaryService>();
        _mapperMock = new Mock<IMapper>();
        _identityGrpcClientMock = new Mock<IIdentityGrpcClient>();
    }

    [Fact]
    public async Task CanSearch()
    {
        //Arrange
        var courseSearchModel = new CourseSearchModel
        {
            PageSize = 20,
            PageIndex = 2,
        };
        var totalItemsCount = 22;

        var expectedItemsInPage = 2;
        var expectedTotalItems = 22;

        _courseRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<CourseSearchModel>()))
            .ReturnsAsync(new PagedList<Course>(new[]
                                                {
                                                    new Course(),
                                                    new Course()
                                                }, courseSearchModel.PageIndex, courseSearchModel.PageSize, totalItemsCount));

        _mapperMock
            .Setup(m => m.Map<List<CourseSearchResultDto>>(It.IsAny<List<Course>>()))
            .Returns(new List<CourseSearchResultDto>(new[] { new CourseSearchResultDto(), new CourseSearchResultDto() }));

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);
        var actionResult = await courseController.Search(courseSearchModel);

        //Assert
        _courseRepositoryMock.Verify(r => r.SearchAsync(It.IsAny<CourseSearchModel>()));
        _mapperMock.Verify(m => m.Map<List<CourseSearchResultDto>>(It.IsAny<List<Course>>()));

        Assert.IsType<ActionResult<PaginationModel<CourseSearchResultDto>>>(actionResult);

        var page = Assert.IsAssignableFrom<PaginationModel<CourseSearchResultDto>>(actionResult.Value);
        Assert.Equal(expectedTotalItems, page.TotalCount);

        Assert.Equal(courseSearchModel.PageIndex, page.PageIndex);
        Assert.Equal(courseSearchModel.PageSize, page.PageSize);
        Assert.Equal(expectedItemsInPage, page.Data.Count());
    }

    [Fact]
    public async Task CanGetCourseDetails()
    {
        //Arrange
        var courseId = 123;
        var course = new Course() {
            Id = courseId, 
            Instructor = new ApplicationUser
            { 
                CreatedCourses = new List<Course>
                {
                    new Course { Id = courseId } 
                }
            } 
        };

        _courseRepositoryMock
            .Setup(r => r.GetCourseDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(m => m.Map<CourseDetailsDto>(It.IsAny<Course>()))
            .Returns(new CourseDetailsDto() {
                Id = courseId,
                Instructor = new InstructorDto
                {
                    CreatedCourses = new List<CourseSearchResultDto>
                    {
                        new CourseSearchResultDto { Id = courseId }
                    }
                }
            });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);
        var actionResult = await courseController.GetCourseDetails(courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetCourseDetailsAsync(It.IsAny<int>()));
        _mapperMock.Verify(m => m.Map<CourseDetailsDto>(It.IsAny<Course>()));
        Assert.Equal(courseId, actionResult?.Value?.Id);
    }

    #region

    [Fact]
    public async Task CanSetintendedlearners()
    {
        //Arrange
        var intentedLearnersDto = new IntendedLearnersDto();
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { InstructorId = userId });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };

        var actionResult = await courseController.SetIntendedLearners(intentedLearnersDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.SetIntendedLearnersAsync(It.IsAny<Course>(), It.IsAny<List<string>>(), 
            It.IsAny<List<string>>(), It.IsAny<List<string>>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanSetCourseLandingPage()
    {
        //Arrange
        var courseLandingPageDto = new CourseLandingPageDto();
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { InstructorId = userId });

        _courseServiceMock
            .Setup(cs => cs.SetCoursePictureAsync(It.IsAny<IFormFile>(), It.IsAny<int>()))
            .ReturnsAsync(string.Empty);

        _courseServiceMock
            .Setup(cs => cs.SetCoursePreviewVideoAsync(It.IsAny<IFormFile>(), It.IsAny<int>()))
            .ReturnsAsync(string.Empty);

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };

        var actionResult = await courseController.SetCourseLandingPage(courseLandingPageDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.SetCourseLandingPageAsync(It.IsAny<Course>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Language?>(), It.IsAny<Level?>(), It.IsAny<string>()));
        _courseServiceMock.Verify(cs => cs.SetCoursePictureAsync(It.IsAny<IFormFile>(), It.IsAny<int>()));
        _courseServiceMock.Verify(cs => cs.SetCoursePreviewVideoAsync(It.IsAny<IFormFile>(), It.IsAny<int>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanSetPrice()
    {
        //Arrange
        var priceDto = new PriceDto();
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { InstructorId = userId });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };

        var actionResult = await courseController.SetPrice(priceDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.SetPriceAsync(It.IsAny<Course>(), It.IsAny<decimal>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    #endregion

    #region Read Coruse Draft

    [Fact]
    public async Task CanGetIntendedLearners()
    {
        //Arrange
        var courseId = 1;
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { Id = courseId, InstructorId = userId });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };
        var actionResult = await courseController.GetIntendedLearners(courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        Assert.Equal(courseId, actionResult?.Value?.CourseId);
    }

    [Fact]
    public async Task CanGetCourseLandingPage()
    {
        //Arrange
        var courseId = 1;
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetCourseByIdAsync(It.IsAny<int>(), true, false))
            .ReturnsAsync(new Course() { Id = courseId, InstructorId = userId });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };
        var actionResult = await courseController.GetCourseLandingPage(courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetCourseByIdAsync(It.IsAny<int>(), true, false));
        Assert.Equal(courseId, actionResult?.Value?.CourseId);
    }

    [Fact]
    public async Task CanGetPrice()
    {
        //Arrange
        var courseId = 1;
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { Id = courseId, InstructorId = userId });

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };
        var actionResult = await courseController.GetPrice(courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        Assert.Equal(courseId, actionResult?.Value?.CourseId);
    }

    [Fact]
    public async Task CanGetCourseCurriculum()
    {
        //Arrange
        var courseId = 1;
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { Id = courseId, InstructorId = userId });

        _courseRepositoryMock
            .Setup(r => r.GetCourseCurriculum(It.IsAny<Course>()))
            .ReturnsAsync(new List<Section>());

        _mapperMock
            .Setup(m => m.Map<List<SectionDto>>(It.IsAny<List<Section>>()))
            .Returns(new List<SectionDto>());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };
        var actionResult = await courseController.GetCourseCurriculum(courseId);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _mapperMock.Verify(m => m.Map<List<SectionDto>>(It.IsAny<List<Section>>()));
        Assert.Equal((int)HttpStatusCode.OK, (actionResult.Result as OkObjectResult)?.StatusCode);
    }

    #endregion

    #region Create Curriculum Draft

    [Fact]
    public async Task CanCreateSectionDraft()
    {
        //Arrange
        var sectionDraftCreateDto = new SectionDraftCreateDto();
        var userId = "userId";

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course() { InstructorId = userId });

        _courseRepositoryMock
            .Setup(r => r.CreateDraftAsync(It.IsAny<Section>()))
            .ReturnsAsync(new Section());

        _mapperMock
            .Setup(m => m.Map<Section>(It.IsAny<SectionDraftCreateDto>()))
            .Returns(new Section());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };

        var actionResult = await courseController.CreateSectionDraft(sectionDraftCreateDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _mapperMock.Verify(m => m.Map<Section>(It.IsAny<SectionDraftCreateDto>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanCreateLectureDraft()
    {
        //Arrange
        var lectureDraftCreateDto = new LectureDraftCreateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Section>(It.IsAny<int>()))
            .ReturnsAsync(new Section());

        _courseRepositoryMock
            .Setup(r => r.CreateDraftAsync(It.IsAny<Lecture>()))
            .ReturnsAsync(new Lecture());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.CreateLectureDraft(lectureDraftCreateDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Section>(It.IsAny<int>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanSetLectureVideoDraft()
    {
        //Arrange
        var lectureVideoDraftDto = new LectureVideoDraftDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()))
            .ReturnsAsync(new Lecture());

        _courseServiceMock
            .Setup(cs => cs.SetLectureVideoAsync(It.IsAny<IFormFile>(), It.IsAny<int>()))
            .ReturnsAsync(string.Empty);

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.SetLectureVideoDraft(lectureVideoDraftDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()));
        _courseServiceMock.Verify(cs => cs.SetLectureVideoAsync(It.IsAny<IFormFile>(), It.IsAny<int>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanCreateLectureVideoSubtitleDraft()
    {
        //Arrange
        var subtitleDraftDto = new SubtitleDraftDto() { File = new FormFile(new MemoryStream(), 0, 0, "id_from_form", "file_name") };

        _courseRepositoryMock
        .Setup(r => r.GetLectureVideoByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new LectureVideo());

        _courseRepositoryMock
        .Setup(r => r.CreateDraftAsync(It.IsAny<Subtitle>()))
            .ReturnsAsync(new Subtitle());

        _courseServiceMock
            .Setup(cs => cs.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.CreateLectureVideoSubtitleDraft(subtitleDraftDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetLectureVideoByIdAsync(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.CreateDraftAsync(It.IsAny<Subtitle>()));
        _courseServiceMock.Verify(cs => cs.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanCreateLectureDownloadableFileDraft()
    {
        //Arrange
        var lectureDownloadableFileDraft = 
            new LectureDownloadableFileDraft() { File = new FormFile(new MemoryStream(), 0, 0, "id_from_form", "file_name") };

        _courseRepositoryMock
        .Setup(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()))
            .ReturnsAsync(new Lecture());

        _courseRepositoryMock
        .Setup(r => r.CreateDraftAsync(It.IsAny<DownloadableFile>()))
            .ReturnsAsync(new DownloadableFile());

        _courseServiceMock
            .Setup(cs => cs.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.CreateLectureDownloadableFileDraft(lectureDownloadableFileDraft) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.CreateDraftAsync(It.IsAny<DownloadableFile>()));
        _courseServiceMock.Verify(cs => cs.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanCreateQuizDraft()
    {
        //Arrange
        var quizDraftCreateDto = new QuizDraftCreateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Section>(It.IsAny<int>()))
            .ReturnsAsync(new Section());

        _courseRepositoryMock
            .Setup(r => r.CreateDraftAsync(It.IsAny<Quiz>()))
            .ReturnsAsync(new Quiz());

        _mapperMock
            .Setup(m => m.Map<Quiz>(It.IsAny<QuizDraftCreateDto>()))
            .Returns(new Quiz());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.CreateQuizDraft(quizDraftCreateDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Section>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.CreateDraftAsync(It.IsAny<Quiz>()));
        _mapperMock.Verify(m => m.Map<Quiz>(It.IsAny<QuizDraftCreateDto>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanCreateQuestionDraft()
    {
        //Arrange
        var questionDraftCreateDto = new QuestionDraftCreateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()))
            .ReturnsAsync(new Quiz());

        _courseRepositoryMock
            .Setup(r => r.CreateDraftAsync(It.IsAny<Question>()))
            .ReturnsAsync(new Question());

        _mapperMock
            .Setup(m => m.Map<Question>(It.IsAny<QuestionDraftCreateDto>()))
            .Returns(new Question());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.CreateQuestionDraft(questionDraftCreateDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.CreateDraftAsync(It.IsAny<Question>()));
        _mapperMock.Verify(m => m.Map<Question>(It.IsAny<QuestionDraftCreateDto>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    #endregion

    #region Update Curriculum

    [Fact]
    public async Task CanUpdateSectionDraft()
    {
        //Arrange
        var sectionDraftUpdateDto = new SectionDraftUpdateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Section>(It.IsAny<int>()))
            .ReturnsAsync(new Section());

        _courseRepositoryMock
            .Setup(r => r.UpdateDraftAsync(It.IsAny<Section>()))
            .ReturnsAsync(new Section());

        _mapperMock
            .Setup(m => m.Map(It.IsAny<SectionDraftUpdateDto>(), It.IsAny<Section>()))
            .Returns(new Section());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.UpdateSectionDraft(sectionDraftUpdateDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Section>(It.IsAny<int>()));
        _mapperMock.Verify(m => m.Map(It.IsAny<SectionDraftUpdateDto>(), It.IsAny<Section>()));
        _courseRepositoryMock.Verify(r => r.UpdateDraftAsync(It.IsAny<Section>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanUpdateLectureDraft()
    {
        //Arrange
        var lectureDraftUpdateDto = new LectureDraftUpdateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()))
            .ReturnsAsync(new Lecture());

        _courseRepositoryMock
            .Setup(r => r.UpdateDraftAsync(It.IsAny<Lecture>()))
            .ReturnsAsync(new Lecture());

        _mapperMock
            .Setup(m => m.Map(It.IsAny<LectureDraftUpdateDto>(), It.IsAny<Lecture>()))
            .Returns(new Lecture());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.UpdateLectureDraft(lectureDraftUpdateDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.UpdateDraftAsync(It.IsAny<Lecture>()));
        _mapperMock.Verify(m => m.Map(It.IsAny<LectureDraftUpdateDto>(), It.IsAny<Lecture>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanUpdateQuizDraft()
    {
        //Arrange
        var quizDraftUpdateDto = new QuizDraftUpdateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()))
            .ReturnsAsync(new Quiz());

        _courseRepositoryMock
            .Setup(r => r.UpdateDraftAsync(It.IsAny<Quiz>()))
            .ReturnsAsync(new Quiz());

        _mapperMock
            .Setup(m => m.Map(It.IsAny<QuizDraftUpdateDto>(), It.IsAny<Quiz>()))
            .Returns(new Quiz());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.UpdateQuizDraft(quizDraftUpdateDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.UpdateDraftAsync(It.IsAny<Quiz>()));
        _mapperMock.Verify(m => m.Map(It.IsAny<QuizDraftUpdateDto>(), It.IsAny<Quiz>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanUpdateQuestionDraft()
    {
        //Arrange
        var questionDraftUpdateDto = new QuestionDraftUpdateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Question>(It.IsAny<int>()))
            .ReturnsAsync(new Question());

        _courseRepositoryMock
            .Setup(r => r.UpdateDraftAsync(It.IsAny<Question>()))
            .ReturnsAsync(new Question());

        _mapperMock
            .Setup(m => m.Map(It.IsAny<QuestionDraftUpdateDto>(), It.IsAny<Question>()))
            .Returns(new Question());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.UpdateQuestionDraft(questionDraftUpdateDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Question>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.UpdateDraftAsync(It.IsAny<Question>()));
        _mapperMock.Verify(m => m.Map(It.IsAny<QuestionDraftUpdateDto>(), It.IsAny<Question>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    #endregion

    #region Delete Curriculum

    [Fact]
    public async Task CanDeleteSection()
    {
        //Arrange
        var sectionId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Section>(It.IsAny<int>()))
            .ReturnsAsync(new Section());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteSection(sectionId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Section>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Section>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanDeleteLecture()
    {
        //Arrange
        var lectureId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()))
            .ReturnsAsync(new Lecture());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteLecture(lectureId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Lecture>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Lecture>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanDeleteDownloadableFile()
    {
        //Arrange
        var fileId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<DownloadableFile>(It.IsAny<int>()))
            .ReturnsAsync(new DownloadableFile());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteDownloadableFile(fileId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<DownloadableFile>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<DownloadableFile>()));
        _courseServiceMock.Verify(cs => cs.DeleteFile(It.IsAny<string>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanDeleteQuiz()
    {
        //Arrange
        var quizId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()))
            .ReturnsAsync(new Quiz());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteQuiz(quizId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Quiz>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Quiz>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanDeleteQuestion()
    {
        //Arrange
        var questionId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Question>(It.IsAny<int>()))
            .ReturnsAsync(new Question());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteQuestion(questionId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Question>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Question>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    #endregion

    #region Ratings

    [Fact]
    public async Task CanRateCourse()
    {
        //Arrange
        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, "userId") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        var ratingCreateDto = new RatingCreateDto();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync<Course>(It.IsAny<int>()))
            .ReturnsAsync(new Course());

        _courseRepositoryMock
            .Setup(r => r.RateCourseAsync(It.IsAny<Rating>()))
            .ReturnsAsync(new Rating());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };
        var actionResult = await courseController.RateCourse(ratingCreateDto) as CreatedAtActionResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetByIdAsync<Course>(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.RateCourseAsync(It.IsAny<Rating>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task CanUpdateRating()
    {
        //Arrange
        var ratingUpdateDto = new RatingUpdateDto();

        _courseRepositoryMock
            .Setup(r => r.GetRatingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Rating());

        _mapperMock
            .Setup(m => m.Map(It.IsAny<RatingUpdateDto>(), It.IsAny<Rating>()))
            .Returns(new Rating());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.UpdateRating(ratingUpdateDto) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetRatingByIdAsync(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.UpdateRatingAsync(It.IsAny<Rating>()));
        _mapperMock.Verify(m => m.Map(It.IsAny<RatingUpdateDto>(), It.IsAny<Rating>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CanDeleteRating()
    {
        //Arrange
        var ratingId = 1;

        _courseRepositoryMock
            .Setup(r => r.GetRatingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Rating());

        //Act
        var courseController = new CourseController(_courseRepositoryMock.Object, _courseServiceMock.Object,
            _cloudinaryServiceMock.Object, _mapperMock.Object, _identityGrpcClientMock.Object);

        var actionResult = await courseController.DeleteRating(ratingId) as NoContentResult;

        //Assert
        _courseRepositoryMock.Verify(r => r.GetRatingByIdAsync(It.IsAny<int>()));
        _courseRepositoryMock.Verify(r => r.DeleteRatingAsync(It.IsAny<Rating>()));
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.NoContent);
    }

    #endregion
}