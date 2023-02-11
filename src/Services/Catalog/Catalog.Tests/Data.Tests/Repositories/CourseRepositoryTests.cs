using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Catalog.Core;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.FilteringAndSorting;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using System.Linq.Expressions;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Data.Tests.Repositories;

public class CourseRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    public CourseRepositoryTests(TestDatabaseFixture fixture)
        => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    #region Search

    [Fact]
    public async Task CanSearchPaginatedAndFiltered()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var searchModel = new CourseSearchModel
        {
            PageIndex = 1,
            PageSize = 2,
            CategoryId = 1,
            Topic = "ASP.NET Core",
            MinimumRating = 1,
            Price = PriceEnum.Paid,
            Level = new Level[] { Level.AllLevels, Level.Beginner, Level.Intermediate, Level.Expert },
            Language = new Language[] { Language.English, Language.Arabic, Language.Spanish },
            Duration = new DurationEnum[]
                { DurationEnum.ExtraShort, DurationEnum.Short, DurationEnum.Medium, DurationEnum.Long, DurationEnum.ExtraLong },
            OrderBy = CourseSortingEnum.Newest
        };

        //Act
        var result = await courseRepository.SearchAsync(searchModel);

        //Assert
        Assert.Equal(searchModel.PageSize, result.Count);
        Assert.Equal(searchModel.PageIndex, result.PageIndex);
    }

    [Theory]
    [InlineData("machine learning")]
    [InlineData("tamer")]
    [InlineData("web development")]
    [InlineData("asp.net")]
    public async Task CanSearchByTerms(string searchTerm)
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var searchModel = new CourseSearchModel
        {
            q = searchTerm
        };

        //Act
        var result = await courseRepository.SearchAsync(searchModel);

        //Assert
        Assert.NotEmpty(result);
    }

    #endregion

    #region Create Course Draft

    [Fact]
    public async Task CanCreateDraft()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var courseToBeCreated = new Course
        {
            Title = "Test course title", 
            InstructorId = "e780a6bd-0826-4a34-8844-b31f3f1469a9",
            SubCategoryId = 1,
            Discount = new()
        };
        Assert.Equal(0, courseToBeCreated.Id);

        //Act
        await courseRepository.CreateDraftAsync(courseToBeCreated);

        context.ChangeTracker.Clear();

        //Assert
        Assert.NotEqual(0, courseToBeCreated.Id);
    }

    [Fact]
    public async Task CanSetIntendedLearners()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var courseToUpdate = await courseRepository.GetCourseByIdAsync(1);
        Assert.NotNull(courseToUpdate);

        var whatYouWillLearn = new List<string> { "Test learning objective 1", "Test learning objective 2" };
        var requirements = new List<string> { "Test requirement 1" };

        //Act
        await courseRepository.SetIntendedLearnersAsync(courseToUpdate!, whatYouWillLearn, requirements, null);

        context.ChangeTracker.Clear();

        //Assert
        Assert.StartsWith(whatYouWillLearn[0], courseToUpdate!.WhatYouWillLearn);
        Assert.Null(courseToUpdate.WhoIsThisCourseFor);
    }

    [Fact]
    public async Task CanSetCourseLandingPage()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var description = "Test description";

        var courseToUpdate = await courseRepository.GetCourseByIdAsync(1);
        Assert.NotNull(courseToUpdate);

        //Act
        await courseRepository.SetCourseLandingPageAsync(courseToUpdate!, null, null, description, null, null, null);

        context.ChangeTracker.Clear();

        //Assert
        Assert.Equal(description, courseToUpdate!.Description);
        Assert.NotNull(courseToUpdate.Title);
        Assert.Null(courseToUpdate.SubTitle);
    }

    [Fact]
    public async Task CanCreateLectureVideoDraftAndIncrementCourseHours()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var lecture = await courseRepository.CreateDraftAsync(new Lecture { SectionId = 1 });
        Assert.NotEqual(0, lecture.Id);

        var section = await courseRepository.GetByIdAsync<Section>(lecture!.SectionId);
        var course = await courseRepository.GetByIdAsync<Course>(section!.CourseId);

        var courseTotalHoursBefore = course!.TotalHours;

        //Act
        var lectureVideo = new LectureVideo() { LectureId = lecture.Id, Duration = 1 };
        Assert.Equal(0, lectureVideo.Id);

        lectureVideo = await courseRepository.CreateLectureVideoDraftAndIncrementCourseHoursAsync(lectureVideo);
        var courseTotalHoursAfter = course.TotalHours;

        context.ChangeTracker.Clear();

        //Assert
        Assert.NotEqual(0, lectureVideo.Id);
        Assert.Equal(courseTotalHoursBefore + lectureVideo.Duration, courseTotalHoursAfter);
    }

    [Fact]
    public async Task CanSetPrice()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var course = await courseRepository.GetByIdAsync<Course>(1);
        Assert.NotNull(course);

        var newPrice = course!.Price + 10;

        //Act
        await courseRepository.SetPriceAsync(course!, newPrice);

        context.ChangeTracker.Clear();

        //Assert
        Assert.Equal(newPrice, course.Price);
    }

    #endregion

    #region Read Course Draft

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        Course? course = await courseRepository.GetCourseByIdAsync(1);

        //Assert
        Assert.NotNull(course);
    }

    [Fact]
    public async Task CanGetLectureVideoById()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        LectureVideo? lectureVideo = await courseRepository.GetLectureVideoByIdAsync(1);

        //Assert
        Assert.NotNull(lectureVideo);
    }

    [Fact]
    public async Task CanGetCourseWithPictureAndVideo()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        var course = await courseRepository.GetCourseByIdAsync(1, includePictureAndVideo: true);

        //Assert
        Assert.NotNull(course);
        Assert.NotNull(course!.Picture);
        Assert.NotNull(course.PreviewVideo);
    }

    [Fact]
    public async Task CanGetCourseDetails()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        //Act & Assert
        var course = await courseRepository.GetCourseDetailsAsync(1);
        Assert.NotNull(course);

        course!.IsPublished = false;
        await courseRepository.UpdateDraftAsync(course);
        course = await courseRepository.GetCourseDetailsAsync(1);
        Assert.Null(course);
    }

    [Fact]
    public async Task CanGetCoursePicture()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        Picture? picture = await courseRepository.GetCoursePictureAsync(1);

        //Assert
        Assert.NotNull(picture);
    }

    [Fact]
    public async Task CanGetCoursePreviewVideo()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        CoursePreviewVideo? video = await courseRepository.GetCoursePreviewVideoAsync(1);

        //Assert
        Assert.NotNull(video);
    }

    [Fact]
    public async Task CanGetCoursePrice()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var existingId = 1;
        var notExistingId = int.MinValue;

        //Act
        decimal? existingPrice = await courseRepository.GetCoursePriceAsync(existingId);
        decimal? notExistingPrice = await courseRepository.GetCoursePriceAsync(notExistingId);

        //Assert
        Assert.NotNull(existingPrice);
        Assert.Null(notExistingPrice);
    }

    #endregion

    [Fact]
    public async Task CanUpdateDraft()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var course = await courseRepository.GetByIdAsync<Course>(1);
        Assert.NotNull(course);
        var oldCourseTitle = course!.Title;

        //Act
        course.Title = "Updated test course";
        await courseRepository.UpdateDraftAsync(course);

        var updatedCourse = await courseRepository.GetByIdAsync<Course>(course.Id);
        Assert.NotNull(updatedCourse);

        context.ChangeTracker.Clear();

        //Assert
        Assert.NotEqual(oldCourseTitle, updatedCourse!.Title);
    }

    [Fact]
    public async Task CanSubmitForAdminReview()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var course = await courseRepository.GetByIdAsync<Course>(8);
        Assert.False(course!.IsUnderAdminReview);

        var sectionsOrders = new Dictionary<int, int>();
        var lecturesOrders = new Dictionary<int, int>();
        var questionsOrders = new Dictionary<int, int>();

        //Act
        await courseRepository.SubmitForAdminReviewAsync(course, sectionsOrders, lecturesOrders, questionsOrders);

        context.ChangeTracker.Clear();

        //Assert
        Assert.True(course.IsUnderAdminReview);
    }

    [Fact]
    public async Task CanDelete()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var course = await courseRepository.GetByIdAsync<Course>(1);
        Assert.NotNull(course);

        //Act
        await courseRepository.DeleteAsync(course!);

        context.ChangeTracker.Clear();

        //Assert
        course = await courseRepository.GetByIdAsync<Course>(course!.Id);
        Assert.Null(course);
    }

    [Fact]
    public async Task CanDeleteLectureVideoAndDecrementCourseHours()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var lectureVideo = await courseRepository.GetLectureVideoByIdAsync(1);
        Assert.NotNull(lectureVideo);

        var lecture = await courseRepository.GetByIdAsync<Lecture>(lectureVideo!.LectureId);
        Assert.NotNull(lecture);

        var section = await courseRepository.GetByIdAsync<Section>(lecture!.SectionId);
        var course = await courseRepository.GetByIdAsync<Course>(section!.CourseId);

        var courseTotalHoursBefore = course!.TotalHours;
        var videoDuration = lectureVideo!.Duration;

        //Act
        await courseRepository.DeleteLectureVideoAndDecrementCourseHoursAsync(lectureVideo!);
        var courseTotalHoursAfter = course.TotalHours;

        context.ChangeTracker.Clear();

        //Assert
        lectureVideo = await courseRepository.GetLectureVideoByIdAsync(lectureVideo!.Id);
        Assert.Null(lectureVideo);
        Assert.Equal(courseTotalHoursBefore - videoDuration, courseTotalHoursAfter);        
    }

    #region Ratings

    [Fact]
    public async Task CanRateCourse()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var ratedCourse = await courseRepository.GetByIdAsync<Course>(1);
        Assert.NotNull(ratedCourse);
        var oldRatingsCount = ratedCourse!.RatingsCount;
        var oldAverageRating = ratedCourse.AverageRating;

        //Act
        var rating = new Rating
        {
            RatingValue = 5,
            Review = "Test Review",
            StudentId = "e780a6bd-0826-4a34-8844-b31f3f1469a9",
            CourseId = ratedCourse!.Id
        };
        Assert.Equal(0, rating.Id);
        await courseRepository.RateCourseAsync(rating);

        context.ChangeTracker.Clear();

        //Assert
        Assert.NotEqual(0, rating.Id);
        Assert.Equal(oldRatingsCount + 1, ratedCourse.RatingsCount);
        Assert.True(rating.RatingValue >= oldAverageRating ? 
            ratedCourse.AverageRating >= oldAverageRating : ratedCourse.AverageRating < oldAverageRating);

        await courseRepository.DeleteRatingAsync(rating);
    }

    [Fact]
    public async Task CanGetRatingById()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        //Act
        Rating? rating = await courseRepository.GetRatingByIdAsync(1);

        //Assert
        Assert.NotNull(rating);
    }

    [Fact]
    public async Task CanUpdateRating()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var rating = await courseRepository.GetRatingByIdAsync(1);
        Assert.NotNull(rating);
        var oldRatingValue = rating!.RatingValue;

        var ratedCourse = await courseRepository.GetByIdAsync<Course>(rating.CourseId);
        Assert.NotNull(ratedCourse);
        var oldAverageRating = ratedCourse!.AverageRating;

        //Act
        rating!.RatingValue -= 1;
        await courseRepository.UpdateRatingAsync(rating);

        var updatedRating = await courseRepository.GetRatingByIdAsync(rating.Id);
        Assert.NotNull(updatedRating);

        context.ChangeTracker.Clear();

        //Assert
        Assert.NotEqual(oldRatingValue, updatedRating!.RatingValue);
        Assert.True(rating.RatingValue >= oldAverageRating ?
            ratedCourse.AverageRating >= oldAverageRating : ratedCourse.AverageRating < oldAverageRating);

        rating!.RatingValue += 1;
        await courseRepository.UpdateRatingAsync(rating);
    }

    [Fact]
    public async Task CanDeleteRating()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var courseRepository = new CourseRepository(context);

        var rating = new Rating
        {
            RatingValue = 5,
            Review = "Test Review",
            StudentId = "e780a6bd-0826-4a34-8844-b31f3f1469a9",
            CourseId = 1
        };
        rating = await courseRepository.RateCourseAsync(rating);
        Assert.NotNull(rating);

        var ratedCourse = await courseRepository.GetByIdAsync<Course>(rating!.CourseId);
        Assert.NotNull(ratedCourse);

        var oldRatingsCount = ratedCourse!.RatingsCount;
        var oldAverageRating = ratedCourse.AverageRating;

        //Act
        await courseRepository.DeleteRatingAsync(rating!);

        context.ChangeTracker.Clear();

        //Assert
        var deletedRating = await courseRepository.GetRatingByIdAsync(rating!.Id);
        Assert.Null(deletedRating);

        Assert.Equal(oldRatingsCount - 1, ratedCourse.RatingsCount);
        Assert.True(rating!.RatingValue >= oldAverageRating ?
            ratedCourse.AverageRating <= oldAverageRating : ratedCourse.AverageRating > oldAverageRating);
    }

    #endregion

    [Fact]
    public async Task CanEnrollStudentInCourse()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var courseRepository = new CourseRepository(context);

        var courseId = 2;
        var student = await context.ApplicationUsers.Include(s => s.EnrolledInCourses).FirstOrDefaultAsync();
        var course = await courseRepository.GetByIdAsync<Course>(courseId);
        Assert.DoesNotContain(course, student!.EnrolledInCourses);

        //Act
        await courseRepository.EnrollStudentInCourseAsync(student.Id, courseId);

        context.ChangeTracker.Clear();

        //Assert
        Assert.Contains(course, student.EnrolledInCourses);
    }
}
