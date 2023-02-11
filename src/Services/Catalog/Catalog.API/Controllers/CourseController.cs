using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.API.Extensions;
using nagiashraf.CoursesApp.Services.Catalog.Core;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;
using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;
using nagiashraf.CoursesApp.Services.Catalog.Services.Courses;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using System.Net;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseService _courseService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly IIdentityGrpcClient _identityGrpcClient;

    public CourseController(ICourseRepository courseRepository, ICourseService courseService, ICloudinaryService cloudinaryService,
        IMapper mapper, IIdentityGrpcClient identityGrpcClient)
    {
        _courseRepository = courseRepository;
        _courseService = courseService;
        _cloudinaryService = cloudinaryService;
        _mapper = mapper;
        _identityGrpcClient = identityGrpcClient;
    }

    [AllowAnonymous]
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedList<CourseSearchResultDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginationModel<CourseSearchResultDto>>> Search([FromQuery] CourseSearchModel courseSearchModel)
    {
        courseSearchModel ??= new();

        var courses = await _courseRepository.SearchAsync(courseSearchModel);

        var data = _mapper.Map<List<CourseSearchResultDto>>(courses.ToList());
        return new PaginationModel<CourseSearchResultDto>(courses.PageIndex, courses.PageSize, courses.TotalCount, courses.TotalPages, data);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CourseDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CourseDto>> GetCourseById(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id, includePictureAndVideo: true, includeCurriculum: true);
        if (course == null)
            return NotFound($"Course with Id {id} not found");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        return _mapper.Map<CourseDto>(course);
    }

    [AllowAnonymous]
    [HttpGet("details/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CourseDetailsDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CourseDetailsDto>> GetCourseDetails(int id)
    {
        var course = await _courseRepository.GetCourseDetailsAsync(id);

        if (course == null)
            return NotFound($"Course with id {id} not found.");

        var courseDetailsDto = _mapper.Map<CourseDetailsDto>(course);
        var instructorCourses = courseDetailsDto.Instructor.CreatedCourses;
        if (instructorCourses == null)
            return NotFound("Course instructor not found");

        courseDetailsDto.Instructor.InstructorRating = instructorCourses.Select(c => c.AverageRating).Average();
        courseDetailsDto.Instructor.RatingsCount = instructorCourses.Select(c => c.RatingsCount).Sum();
        courseDetailsDto.Instructor.CoursesCount = instructorCourses.Count;
        courseDetailsDto.Instructor.StudentsCount = instructorCourses.Select(c => c.StudentsCount).Sum();

        return courseDetailsDto;
    }

    #region Create and Edit Course Draft

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateCourseDraft(CourseDraftCreateDto courseDraftCreateDto)
    {
        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId))
            return NotFound("User not found");

        if (!await _courseRepository.UserExitsInCatalogServiceAsync(instructorId))
        {
            var user = await _identityGrpcClient.GetUserDetailsAsync(instructorId);
            if (user == null)
                return NotFound("User not found");

            await _courseRepository.CreateUserAsync(user);
        }

        var course = await _courseRepository.CreateDraftAsync(new Course
        {
            InstructorId = instructorId,
            Title = courseDraftCreateDto.CourseTitle,
            SubCategoryId = courseDraftCreateDto.SubcategoryId,
            Discount = new()
        });

        return CreatedAtAction(nameof(GetCourseById), new { course.Id }, null);
    }

    [HttpPost("intended-learners")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> SetIntendedLearners(IntendedLearnersDto intendedLearnersDto)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(intendedLearnersDto.CourseId);
        if (course == null)
            return NotFound($"Course with id {intendedLearnersDto.CourseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        await _courseRepository.SetIntendedLearnersAsync(
        course, 
        intendedLearnersDto.WhatYouWillLearn,
        intendedLearnersDto.Requirements,
        intendedLearnersDto.WhoIsThisCourseFor);

        return NoContent();
    }

    [HttpPost("landing-page")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SetCourseLandingPage([FromForm] CourseLandingPageDto courseLandingPageDto)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(courseLandingPageDto.CourseId);
        if (course == null)
            return NotFound($"Course with id {courseLandingPageDto.CourseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        var pictureErrorMessage = await _courseService.SetCoursePictureAsync(courseLandingPageDto.Picture, course.Id);            
        var videoErrorMessage = await _courseService.SetCoursePreviewVideoAsync(courseLandingPageDto.PreviewVideo, course.Id);

        var joinedErrorMessage = string.Join("\n", new string?[] { pictureErrorMessage, videoErrorMessage });
        if (!string.IsNullOrEmpty(pictureErrorMessage) && !string.IsNullOrEmpty(videoErrorMessage))
            return BadRequest(joinedErrorMessage);

        await _courseRepository.SetCourseLandingPageAsync(course, courseLandingPageDto.Title, courseLandingPageDto.SubTitle,
            courseLandingPageDto.Description, courseLandingPageDto.Language, courseLandingPageDto.Level, courseLandingPageDto.Topic);

        return NoContent();
    }

    [HttpPost("price")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> SetPrice(PriceDto setPriceDto)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(setPriceDto.CourseId);
        if (course == null)
            return NotFound($"Course with id {setPriceDto.CourseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        await _courseRepository.SetPriceAsync(course, setPriceDto.Price);

        return NoContent();
    }

    #endregion

    #region Read Coruse Draft

    [HttpGet("intended-learners/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IntendedLearnersDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IntendedLearnersDto>> GetIntendedLearners(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(courseId);
        if (course == null)
            return NotFound($"Course with id {courseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        return new IntendedLearnersDto
        {
            CourseId = course.Id,
            WhatYouWillLearn = course.WhatYouWillLearn?.ToListBySeparator(),
            Requirements = course.Requirements?.ToListBySeparator(),
            WhoIsThisCourseFor = course.WhoIsThisCourseFor?.ToListBySeparator()
        };
    }

    [HttpGet("landing-page/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CourseLandingPageToReturnDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CourseLandingPageToReturnDto>> GetCourseLandingPage(int courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId, includePictureAndVideo: true);
        if (course == null)
            return NotFound($"Course with id {courseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        return new CourseLandingPageToReturnDto
        {
            CourseId = course.Id,
            Title = course.Title,
            SubTitle = course.SubTitle,
            Description = course.Description,
            Language = course.Language.ToString(),
            Level = course.Level.ToString(),
            Topic = course.Topic,
            PicturePublicId = course.Picture?.PublicId,
            PictureUrl = course.Picture?.Url,
            PreviewVideoPublicId = course.PreviewVideo?.PublicId,
            PreviewVideoUrl = course.PreviewVideo?.Url
        };
    }

    [HttpGet("price/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PriceDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PriceDto>> GetPrice(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(courseId);
        if (course == null)
            return NotFound($"Course with id {courseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        return new PriceDto
        {
            CourseId = course.Id,
            Price = course.Price
        };
    }

    [HttpGet("curriculum/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(List<Section>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<SectionDto>?>> GetCourseCurriculum(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(courseId);
        if (course == null)
            return NotFound($"Course with id {courseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        var sections = await _courseRepository.GetCourseCurriculum(course);

        return Ok(_mapper.Map<List<SectionDto>?>(sections));
    }

    #endregion

    #region Create Curriculum Draft

    [HttpPost("section")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateSectionDraft(SectionDraftCreateDto sectionDraftCreateDto)
    {
        var course = await _courseRepository.GetByIdAsync<Course>(sectionDraftCreateDto.CourseId);
        if (course == null)
            return NotFound($"Course with id {sectionDraftCreateDto.CourseId} not found.");

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        var section = await _courseRepository.CreateDraftAsync(_mapper.Map<Section>(sectionDraftCreateDto));

        return CreatedAtAction(null, new { Id = section.Id }, null);
    }

    [HttpPost("lecture")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateLectureDraft(LectureDraftCreateDto lectureDraftCreateDto)
    {
        var section = await _courseRepository.GetByIdAsync<Section>(lectureDraftCreateDto.SectionId);
        if (section == null)
            return NotFound($"Lecture with id {lectureDraftCreateDto.SectionId} not found.");

        var lecture = await _courseRepository.CreateDraftAsync(new Lecture
        {
            SectionId = lectureDraftCreateDto.SectionId,
            Title = lectureDraftCreateDto.Title
        });

        return CreatedAtAction(null, new { Id = lecture.Id }, null);
    }

    [HttpPost("lecture-video")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SetLectureVideoDraft([FromForm] LectureVideoDraftDto lectureVideoDraftDto)
    {
        var lecture = await _courseRepository.GetByIdAsync<Lecture>(lectureVideoDraftDto.LectureId);
        if (lecture == null)
            return NotFound($"Lecture with id {lectureVideoDraftDto.LectureId} not found.");

        var errorMessage = await _courseService.SetLectureVideoAsync(lectureVideoDraftDto.VideoFile, lecture.Id);
        if (!string.IsNullOrEmpty(errorMessage))
            return BadRequest(errorMessage);

        return NoContent();
    }

    [HttpPost("lecture-video-subtitle")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateLectureVideoSubtitleDraft([FromForm] SubtitleDraftDto subtitleDraftDto)
    {
        var lectureVideo = await _courseRepository.GetLectureVideoByIdAsync(subtitleDraftDto.VideoId);
        if (lectureVideo == null)
            return NotFound($"Lecture video with id {subtitleDraftDto.VideoId} not found.");

        if (subtitleDraftDto.File == null)
            return BadRequest("No file uploaded");

        var uploadedFilePath = await _courseService.UploadFileAsync(subtitleDraftDto.File, "subtitles");

        var subtitle = await _courseRepository.CreateDraftAsync(new Subtitle
        {
            VideoId = subtitleDraftDto.VideoId,
            FilePath = uploadedFilePath,
            Language = subtitleDraftDto.Language
        });

        return CreatedAtAction(null, new { Id = subtitle.Id }, null); ;
    }

    [HttpPost("lecture-downloadable-file")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateLectureDownloadableFileDraft([FromForm] LectureDownloadableFileDraft lectureDownloadableFileDraft)
    {
        var lecture = await _courseRepository.GetByIdAsync<Lecture>(lectureDownloadableFileDraft.LectureId);
        if (lecture == null)
            return NotFound($"Lecture with id {lectureDownloadableFileDraft.LectureId} not found.");

        if (lectureDownloadableFileDraft.File == null)
            return BadRequest("No file uploaded");

        var uploadedFileUrlPath = await _courseService.UploadFileAsync(lectureDownloadableFileDraft.File, "downloadableFiles");

        var downloadableFile = await _courseRepository.CreateDraftAsync(new DownloadableFile
        {
            LectureId = lectureDownloadableFileDraft.LectureId,
            FileUrlPath = uploadedFileUrlPath
        });

        return CreatedAtAction(null, new { Id = downloadableFile.Id }, null);
    }

    [HttpPost("quiz")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateQuizDraft(QuizDraftCreateDto quizDraftCreateDto)
    {
        var section = await _courseRepository.GetByIdAsync<Section>(quizDraftCreateDto.SectionId);
        if (section == null)
            return NotFound($"Section with id {quizDraftCreateDto.SectionId} not found.");

        var quiz = await _courseRepository.CreateDraftAsync(_mapper.Map<Quiz>(quizDraftCreateDto));

        return CreatedAtAction(null, new { Id = quiz.Id }, null);
    }

    [HttpPost("question")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateQuestionDraft(QuestionDraftCreateDto questionDraftCreateDto)
    {
        var quiz = await _courseRepository.GetByIdAsync<Quiz>(questionDraftCreateDto.QuizId);
        if (quiz == null)
            return NotFound($"Quiz with id {questionDraftCreateDto.QuizId} not found.");

        var question = await _courseRepository.CreateDraftAsync(_mapper.Map<Question>(questionDraftCreateDto));

        return CreatedAtAction(null, new { Id = question.Id }, null);
    }

    #endregion

    #region Update Curriculum

    [HttpPut("section")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateSectionDraft(SectionDraftUpdateDto sectionDraftUpdateDto)
    {
        var section = await _courseRepository.GetByIdAsync<Section>(sectionDraftUpdateDto.Id);
        if (section == null)
            return NotFound($"Section with id {sectionDraftUpdateDto.Id} not found.");

        await _courseRepository.UpdateDraftAsync(_mapper.Map(sectionDraftUpdateDto, section));

        return NoContent();
    }

    [HttpPut("lecture")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateLectureDraft(LectureDraftUpdateDto lectureDraftUpdateDto)
    {
        var lecture = await _courseRepository.GetByIdAsync<Lecture>(lectureDraftUpdateDto.Id);
        if (lecture == null)
            return NotFound($"Lecture with id {lectureDraftUpdateDto.Id} not found.");

        await _courseRepository.UpdateDraftAsync(_mapper.Map(lectureDraftUpdateDto, lecture));

        return NoContent();
    }

    [HttpPut("quiz")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateQuizDraft(QuizDraftUpdateDto quizDraftUpdateDto)
    {
        var quiz = await _courseRepository.GetByIdAsync<Quiz>(quizDraftUpdateDto.Id);
        if (quiz == null)
            return NotFound($"Quiz with id {quizDraftUpdateDto.Id} not found.");

        await _courseRepository.UpdateDraftAsync(_mapper.Map(quizDraftUpdateDto, quiz));

        return NoContent();
    }

    [HttpPut("question")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateQuestionDraft(QuestionDraftUpdateDto questionDraftUpdateDto)
    {
        var question = await _courseRepository.GetByIdAsync<Question>(questionDraftUpdateDto.Id);
        if (question == null)
            return NotFound($"Question with id {questionDraftUpdateDto.Id} not found.");

        await _courseRepository.UpdateDraftAsync(_mapper.Map(questionDraftUpdateDto, question));

        return NoContent();
    }

    #endregion

    #region Delete Curriculum

    [HttpDelete("section/{sectionId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteSection(int sectionId)
    {
        var section = await _courseRepository.GetByIdAsync<Section>(sectionId);
        if (section == null)
            return NotFound($"Section with id {sectionId} not found.");

        await _courseRepository.DeleteAsync(section);

        return NoContent();
    }

    [HttpDelete("lecture/{lectureId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteLecture(int lectureId)
    {
        var lecture = await _courseRepository.GetByIdAsync<Lecture>(lectureId);
        if (lecture == null)
            return NotFound($"Lecture with id {lectureId} not found.");

        await _courseRepository.DeleteAsync(lecture);

        return NoContent();
    }

    [HttpDelete("lecture-downloadable-file/{fileId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteDownloadableFile(int fileId)
    {
        var file = await _courseRepository.GetByIdAsync<DownloadableFile>(fileId);
        if (file == null)
            return NotFound($"File with id {fileId} not found.");

        _courseService.DeleteFile(Path.Combine("downloadableFiles", file.FileUrlPath));

        await _courseRepository.DeleteAsync(file);

        return NoContent();
    }

    [HttpDelete("quiz/{quizId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteQuiz(int quizId)
    {
        var quiz = await _courseRepository.GetByIdAsync<Quiz>(quizId);
        if (quiz == null)
            return NotFound($"Quiz with id {quizId} not found.");

        await _courseRepository.DeleteAsync(quiz);

        return NoContent();
    }

    [HttpDelete("question/{questionId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        var question = await _courseRepository.GetByIdAsync<Question>(questionId);
        if (question == null)
            return NotFound($"Question with id {questionId} not found.");

        await _courseRepository.DeleteAsync(question);

        return NoContent();
    }

    #endregion

    [HttpPost("submit")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> SubmitForAdminReview(CourseDto courseDto)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseDto.Id);
        if (course == null)
            return NotFound();

        var instructorId = User.GetIdFromPrincipal();
        if (string.IsNullOrEmpty(instructorId) || instructorId != course.InstructorId)
            return Forbid();

        var sectionsOrders = courseDto.Sections
            .ToDictionary(s => s.Id, s => s.Order);
        var lecturesOrders = courseDto.Sections
            .SelectMany(s => s.Lectures)
            .ToDictionary(l => l.Id, l => l.Order);
        var QuestionsOrders = courseDto.Sections
            .SelectMany(s => s.Quizzes)
            .SelectMany(qz => qz.Questions)
            .ToDictionary(qn => qn.Id, qn => qn.Order);

        await _courseRepository.SubmitForAdminReviewAsync(course, sectionsOrders, lecturesOrders, QuestionsOrders);

        return NoContent();
    }

    #region Ratings

    [HttpPost("rating")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RateCourse(RatingCreateDto ratingCreateDto)
    {
        var studentId = User.GetIdFromPrincipal();
        if (studentId == null)
            return NotFound("User not found.");

        var course = await _courseRepository.GetByIdAsync<Course>(ratingCreateDto.CourseId);
        if (course == null)
            return NotFound($"Course with id {ratingCreateDto.CourseId} not found.");

        var rating = await _courseRepository.RateCourseAsync(new Rating
        {
            RatingValue = ratingCreateDto.RatingValue,
            Review = ratingCreateDto.Review,
            StudentId = studentId,
            CourseId = course.Id
        });
        if (rating == null)
            return BadRequest("Problem saving your rating");

        return CreatedAtAction(null, new { Id = rating.Id }, null);
    }

    [HttpPut("rating")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateRating(RatingUpdateDto ratingUpdateDto)
    {
        var rating = await _courseRepository.GetRatingByIdAsync(ratingUpdateDto.Id);
        if (rating == null)
            return NotFound($"Rating with id {ratingUpdateDto.Id} not found.");

        await _courseRepository.UpdateRatingAsync(_mapper.Map(ratingUpdateDto, rating));

        return NoContent();
    }

    [HttpDelete("rating")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteRating(int ratingId)
    {
        var rating = await _courseRepository.GetRatingByIdAsync(ratingId);
        if (rating == null)
            return NotFound($"Rating with id {ratingId} not found.");

        await _courseRepository.DeleteRatingAsync(rating);

        return NoContent();
    }

    #endregion
}