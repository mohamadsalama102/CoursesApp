using nagiashraf.CoursesApp.Services.Catalog.Core;
using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;
using nagiashraf.CoursesApp.Services.Catalog.Core.FilteringAndSorting;
using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

namespace nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseContext _courseContext;
    public CourseRepository(CourseContext courseContext)
        => _courseContext = courseContext;

    #region Search

    public async Task<PagedList<Course>> SearchAsync(CourseSearchModel courseSearchModel)
    {
        IQueryable<Course> courses = _courseContext.Courses
            .AsNoTracking()
            .Where(c => c.IsPublished);

        courses = Search(courses, courseSearchModel.q);

        courses = Filter(
            courses,
            courseSearchModel.CategoryId,
            courseSearchModel.Topic,
            courseSearchModel.MinimumRating,
            courseSearchModel.Price,
            courseSearchModel.Level,
            courseSearchModel.Language,
            courseSearchModel.Duration,
            courseSearchModel.SubtitleLanguage,
            courseSearchModel.Features);

        courses = Sort(courses, courseSearchModel.OrderBy);

        courses = courses.Include(c => c.Discount)
            .Include(c => c.Picture)
            .Include(c => c.Instructor);

        return await courses.ToPagedListAsync(courseSearchModel.PageIndex, courseSearchModel.PageSize);
    }

    private IQueryable<Course> Search(IQueryable<Course> query, string? searchTerm)
    {        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchTerm = searchTerm.RemoveExtraWhiteSpaces();

            query = query.Where(c => c.Title.Contains(searchTerm)
                || (!string.IsNullOrEmpty(c.SubTitle) && c.SubTitle.Contains(searchTerm))
                || (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(searchTerm))
                || (!string.IsNullOrEmpty(c.WhatYouWillLearn) && c.WhatYouWillLearn.Contains(searchTerm))
                || (!string.IsNullOrEmpty(c.Requirements) && c.Requirements.Contains(searchTerm))
                || (!string.IsNullOrEmpty(c.WhoIsThisCourseFor) && c.WhoIsThisCourseFor.Contains(searchTerm))
                || (c.Instructor.FirstName + " " + c.Instructor.LastName).Contains(searchTerm)
                || c.SubCategory.Name.Contains(searchTerm)
                || c.SubCategory.Category.Name.Contains(searchTerm)
                || (!string.IsNullOrEmpty(c.Topic) && c.Topic.Contains(searchTerm)));
        }
        return query;
    }

    private IQueryable<Course> Filter(
        IQueryable<Course> query,
        int? categoryId,
        string? topic,
        double? minimumRating,
        PriceEnum? price,
        Level[]? level,
        Language[]? language,
        DurationEnum[]? duration,
        Language[]? subtitleLanguage,
        CourseFeaturesEnum[]? courseFeatures)
    {
        if (categoryId != null)
        {
            query = query.Where(c => c.SubCategory.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(topic))
        {
            query = query.Where(c => !string.IsNullOrWhiteSpace(c.Topic) && c.Topic == topic);
        }

       if (minimumRating != null)
        {
            query = query.Where(c => c.AverageRating >= minimumRating);
        }

        if (price != null)
        {
            query = price switch
            {
                PriceEnum.Free => query.Where(c => c.Price == 0),
                PriceEnum.Paid => query.Where(c => c.Price > 0),
                _ => query
            };
        }

        if (level != null && level.Length > 0)
        {
            HashSet<Level> selectedLevels = level.ToHashSet();
            query = query.Where(c => c.Level != null && selectedLevels.Contains(c.Level.Value));
        }

        if (language != null && language.Length > 0)
        {
            HashSet <Language> selectedLanguages = language.ToHashSet();
            query = query.Where(c => c.Language != null && selectedLanguages.Contains(c.Language.Value));
        }

        if (duration != null && duration.Length > 0)
        {
            HashSet<DurationEnum> selectedDurations = duration.ToHashSet();
            query = query.Where(c => (selectedDurations.Contains(DurationEnum.ExtraShort) && c.TotalHours >= 0 && c.TotalHours <= 1)
                || (selectedDurations.Contains(DurationEnum.Short) && c.TotalHours > 1 && c.TotalHours <= 3)
                || (selectedDurations.Contains(DurationEnum.Medium) && c.TotalHours > 3 && c.TotalHours <= 6)
                || (selectedDurations.Contains(DurationEnum.Long) && c.TotalHours > 6 && c.TotalHours <= 17)
                || (selectedDurations.Contains(DurationEnum.ExtraLong) && c.TotalHours > 17));
        }

        if (subtitleLanguage != null && subtitleLanguage.Length > 0)
        {
            HashSet<Language> selectedSubtitleLanguages = subtitleLanguage.ToHashSet();

            IQueryable<int> courseIdsOfSubtitleLanguages = _courseContext.Subtitles
                .Where(sl => selectedSubtitleLanguages.Contains(sl.Language))
                .Select(sl => sl.Video.Lecture.Section)
                .GroupBy(s => s.CourseId)
                .Select(s => s.Key);

            query = query.Join(courseIdsOfSubtitleLanguages,
                c => c.Id,
                id => id,
                (c, id) => c);
        }

        if (courseFeatures != null && courseFeatures.Length > 0)
        {
            HashSet<CourseFeaturesEnum> selectedFeatures = courseFeatures.ToHashSet();
            IQueryable<int> courseIdsOfFeatures = Enumerable.Empty<int>().AsQueryable();

            if (selectedFeatures.Contains(CourseFeaturesEnum.HasQuizzes))
            {
                courseIdsOfFeatures = _courseContext.Quizzes
                    .Select(sl => sl.Section)
                    .GroupBy(s => s.CourseId)
                    .Select(s => s.Key);
            }

            if (selectedFeatures.Contains(CourseFeaturesEnum.HasSubtitles))
            {
                courseIdsOfFeatures = courseIdsOfFeatures.Union(_courseContext.Subtitles
                    .Select(sl => sl.Video.Lecture.Section)
                    .GroupBy(s => s.CourseId)
                    .Select(s => s.Key));
            }

            query = query.Join(courseIdsOfFeatures,
                c => c.Id,
                id => id,
                (c, id) => c);
        }

        return query;
    }

    private IQueryable<Course> Sort(IQueryable<Course> query, CourseSortingEnum? orderBy)
    {
        query = orderBy switch
        {
            CourseSortingEnum.Newest => query.OrderByDescending(c => c.CreatedOn),
            CourseSortingEnum.HighestRated => query.OrderByDescending(c => c.AverageRating),
            _ => query.OrderByDescending(c => c.StudentsCount)
        };

        return query;
    }

    public async Task<List<Course>> GetCoursesUnderReviewAsync()
    {
        return await _courseContext.Courses
            .AsNoTracking()
            .AsSplitQuery()
            .Where(c => c.IsUnderAdminReview)
            .Include(c => c.Picture)
            .Include(c => c.PreviewVideo)
            .Include(c => c.Instructor)
            .Include(c => c.Sections.OrderBy(s => s.Order))
            .ThenInclude(s => s.Lectures.OrderBy(l => l.Order))
            .ThenInclude(l => l.Video)
            .ThenInclude(v => v.Subtitles)
            .Include(c => c.Sections)
            .ThenInclude(s => s.Lectures)
            .ThenInclude(l => l.DownloadableFiles)
            .Include(c => c.Sections)
            .ThenInclude(s => s.Quizzes)
            .ThenInclude(q => q.Questions.OrderBy(q => q.Order))
            .ThenInclude(q => q.Answers.OrderBy(a => a.Order))
            .OrderBy(c => c.CreatedOn)
            .ToListAsync();
    }

    #endregion

    #region Create Course

    public async Task<TEntity> CreateDraftAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        _courseContext.Set<TEntity>().Add(entity);
        await _courseContext.SaveChangesAsync();
        return entity;
    }

    public async Task SetIntendedLearnersAsync(Course course, List<string>? whatYouWillLearn, List<string>? requirements, 
        List<string>? whoIsThisCourseFor)
    {
        course.WhatYouWillLearn = whatYouWillLearn?.ToStringWithSeparator();
        course.Requirements = requirements?.ToStringWithSeparator();
        course.WhoIsThisCourseFor = whoIsThisCourseFor?.ToStringWithSeparator();

        await _courseContext.SaveChangesAsync();
    }

    public async Task SetCourseLandingPageAsync(Course course, string? title, string? subTitle, string? description,
        Language? language, Level? level, string? topic)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            course.Title = title;
        }    

        course.SubTitle = subTitle;
        course.Description = description;
        course.Language = language;
        course.Level = level;
        course.Topic = topic;

        await _courseContext.SaveChangesAsync();
    }

    public async Task<LectureVideo> CreateLectureVideoDraftAndIncrementCourseHoursAsync(LectureVideo lectureVideo)
    {
        _courseContext.LectureVideos.Add(lectureVideo);

        var course = await _courseContext.Lectures
            .Include(l => l.Section)
            .ThenInclude(s => s.Course)
            .Where(l => l.Id == lectureVideo.LectureId)
            .Select(l => l.Section.Course)
            .SingleAsync();
            
        course.TotalHours += lectureVideo.Duration;

        await _courseContext.SaveChangesAsync();

        return lectureVideo;
    }

    public async Task SetPriceAsync(Course course, decimal price)
    {
        course.Price = price;
        await _courseContext.SaveChangesAsync();
    }

    #endregion

    #region Read Course Draft

    public async Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : BaseEntity
    {
        return await _courseContext.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<LectureVideo?> GetLectureVideoByIdAsync(int id)
    {
        return await _courseContext.LectureVideos.SingleOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Course?> GetCourseByIdAsync(int id, bool includePictureAndVideo = false, bool includeCurriculum = false)
    {
        IQueryable<Course> result = _courseContext.Courses.AsSplitQuery();

        if (includePictureAndVideo)
        {
            result = result
                .Include(c => c.Picture)
                .Include(c => c.PreviewVideo);
        }

        if (includeCurriculum)
        {
            result = result
                .Include(c => c.Sections.OrderBy(s => s.Order))
                .ThenInclude(s => s.Lectures.OrderBy(l => l.Order))
                .ThenInclude(l => l.Video)
                .ThenInclude(v => v.Subtitles)
                .Include(c => c.Sections)
                .ThenInclude(s => s.Lectures)
                .ThenInclude(l => l.DownloadableFiles)
                .Include(c => c.Sections)
                .ThenInclude(s => s.Quizzes)
                .ThenInclude(q => q.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.Answers.OrderBy(a => a.Order));
        }

        return await result.SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course?> GetCourseDetailsAsync(int id)
    {
        return await _courseContext.Courses
            .AsSplitQuery()
            .Where(c => c.IsPublished)
            .Include(c => c.Picture)
            .Include(c => c.PreviewVideo)
            .Include(c => c.Sections.OrderBy(s => s.Order))
            .ThenInclude(s => s.Lectures.OrderBy(l => l.Order))
            .Include(c => c.Sections)
            .ThenInclude(s => s.Quizzes)
            .Include(c => c.SubCategory)
            .ThenInclude(sc => sc.Category)
            .Include(c => c.Discount)
            .Include(c => c.Instructor)
            .ThenInclude(i => i.CreatedCourses).Where(c => c.IsPublished)
            .Include(c => c.Ratings)
            .Include(c => c.StudentCourses)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Picture?> GetCoursePictureAsync(int courseId)
    {
        return await _courseContext.Pictures.SingleOrDefaultAsync(p => p.CourseId == courseId);
    }

    public async Task<CoursePreviewVideo?> GetCoursePreviewVideoAsync(int courseId)
    {
        return await _courseContext.CoursePreviewVideos.SingleOrDefaultAsync(v => v.CourseId == courseId);
    }

    public async Task<decimal?> GetCoursePriceAsync(int courseId)
    {
        var course = await GetCourseByIdAsync(courseId);

        if (course == null)
            return null;

        return await _courseContext.Courses.Where(c => c.Id == course.Id)
            .Select(c => c.Price)
            .SingleOrDefaultAsync();
    }

    public async Task<List<Section>?> GetCourseCurriculum(Course course)
    {
        return await _courseContext.Sections
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Lectures.OrderBy(l => l.Order))
            .ThenInclude(l => l.Video)
            .ThenInclude(v => v.Subtitles)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.DownloadableFiles)
            .Include(s => s.Quizzes)
            .ThenInclude(q => q.Questions.OrderBy(q => q.Order))
            .ThenInclude(q => q.Answers.OrderBy(a => a.Order))
            .Where(s => s.CourseId == course.Id)
            .OrderBy(s => s.Order)
            .ToListAsync();
    }

    #endregion

    public async Task<TEntity> UpdateDraftAsync<TEntity>(TEntity entityToUpdate) where TEntity : BaseEntity
    {
        var entity = _courseContext.Set<TEntity>().Attach(entityToUpdate);
        entity.State = EntityState.Modified;
        await _courseContext.SaveChangesAsync();
        return entityToUpdate;
    }

    public async Task<bool> UserExitsInCatalogServiceAsync(string userId)
    {
        return await _courseContext.ApplicationUsers.AnyAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
    {
        _courseContext.ApplicationUsers.Add(user);
        await _courseContext.SaveChangesAsync();
        return user;
    }

    public async Task SubmitForAdminReviewAsync(Course course, Dictionary<int, int> sectionsOrders, Dictionary<int, int> lecturesOrders,
        Dictionary<int, int> questionsOrders)
    {
        await SaveOrdersAsync(course.Id, sectionsOrders, lecturesOrders, questionsOrders);

        course.IsPublished = false;
        course.IsRejected = false;
        course.IsUnderAdminReview = true;

        await _courseContext.SaveChangesAsync();
    }

    private async Task SaveOrdersAsync(int courseId, Dictionary<int, int> sectionsOrders, Dictionary<int, int> lecturesOrders,
        Dictionary<int, int> questionsOrders)
    {
        var sections = await _courseContext.Sections.Where(s => s.CourseId == courseId).ToListAsync();
        sections.ForEach(s => s.Order = sectionsOrders[s.Id]);

        var sectionIds = new HashSet<int>(sections.Select(s => s.Id));

        var lectures = await _courseContext.Lectures.Where(l => sectionIds.Contains(l.SectionId)).ToListAsync();
        lectures.ForEach(l => l.Order = lecturesOrders[l.Id]);

        var questions = await _courseContext.Questions.Where(q => sectionIds.Contains(q.Quiz.SectionId)).ToListAsync();
        questions.ForEach(q => q.Order = questionsOrders[q.Id]);
    }

    public async Task PublishCourseAsync(Course course)
    {
        course.IsUnderAdminReview = false;
        course.IsRejected = false;
        course.IsPublished = true;
        await _courseContext.SaveChangesAsync();
    }

    public async Task RejectCourseAsync(Course course)
    {
        course.IsUnderAdminReview = false;
        course.IsPublished = false;
        course.IsRejected = true;
        await _courseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        _courseContext.Set<TEntity>().Remove(entity);
        await _courseContext.SaveChangesAsync();
    }

    public async Task DeleteLectureVideoAndDecrementCourseHoursAsync(LectureVideo video)
    {
        _courseContext.LectureVideos.Remove(video);

        var course = video.Lecture.Section.Course;
        course.TotalHours -= video.Duration;

        await _courseContext.SaveChangesAsync();
    }

    #region Ratings

    public async Task<Rating?> RateCourseAsync(Rating rating)
    {
        using var transaction = await _courseContext.Database.BeginTransactionAsync();

        try
        {
            _courseContext.Ratings.Add(rating);
            await _courseContext.SaveChangesAsync();

            var course = await _courseContext.Courses.SingleAsync(c => c.Id == rating.CourseId);
            course.RatingsCount++;
            course.AverageRating = await _courseContext.Ratings
                .Where(r => r.CourseId == course.Id)
                .Select(r => r.RatingValue)
                .AverageAsync();
            await _courseContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return rating;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<Rating?> GetRatingByIdAsync(int id)
    {
        return await _courseContext.Ratings.SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task UpdateRatingAsync(Rating ratingToUpdate)
    {
        using var transaction = await _courseContext.Database.BeginTransactionAsync();

        try
        {
            var rating = _courseContext.Ratings.Attach(ratingToUpdate);
            rating.State = EntityState.Modified;

            await _courseContext.SaveChangesAsync();

            var course = await _courseContext.Courses.SingleAsync(c => c.Id == ratingToUpdate.CourseId);
            course.AverageRating = await _courseContext.Ratings
                .Where(r => r.CourseId == course.Id)
                .Select(r => r.RatingValue)
                .AverageAsync();

            await _courseContext.SaveChangesAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task DeleteRatingAsync(Rating rating)
    {
        using var transaction = await _courseContext.Database.BeginTransactionAsync();

        try
        {
            _courseContext.Ratings.Remove(rating);
            await _courseContext.SaveChangesAsync();

            var course = await _courseContext.Courses.SingleAsync(c => c.Id == rating.CourseId);
            course.RatingsCount--;
            course.AverageRating = await _courseContext.Ratings
                .Where(r => r.CourseId == course.Id)
                .Select(r => r.RatingValue)
                .AverageAsync();
            await _courseContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }
    }

    #endregion

    public async Task EnrollStudentInCourseAsync(string studentId, int courseId)
    {
        var course = await GetByIdAsync<Course>(courseId);

        if (course != null)
        {
            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId
            };

            _courseContext.StudentCourses.Add(studentCourse);

            course.StudentsCount++;

            await _courseContext.SaveChangesAsync();
        }
    }
}
