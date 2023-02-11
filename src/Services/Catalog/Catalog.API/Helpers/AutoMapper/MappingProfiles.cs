using AutoMapper;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Helpers.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.WhatYouWillLearn, options => options
                    .MapFrom(src => src.WhatYouWillLearn!.ToListBySeparator("^^SEPARATOR^^")))
                .ForMember(dest => dest.Requirements, options => options
                    .MapFrom(src => src.Requirements!.ToListBySeparator("^^SEPARATOR^^")))
                .ForMember(dest => dest.WhoIsThisCourseFor, options => options
                    .MapFrom(src => src.WhoIsThisCourseFor!.ToListBySeparator("^^SEPARATOR^^")))
                .ReverseMap();
            CreateMap<Course, CourseSearchResultDto>()
                .ForMember(dest => dest.InstructorName, options => options
                    .MapFrom(src => src.Instructor.FirstName + " " + src.Instructor.LastName));
            CreateMap<Course, CourseDetailsDto>()
                .ForMember(dest => dest.WhatYouWillLearn, options => options
                    .MapFrom(src => src.WhatYouWillLearn!.ToListBySeparator("^^SEPARATOR^^")))
                .ForMember(dest => dest.Requirements, options => options
                    .MapFrom(src => src.Requirements!.ToListBySeparator("^^SEPARATOR^^")))
                .ForMember(dest => dest.WhoIsThisCourseFor, options => options
                    .MapFrom(src => src.WhoIsThisCourseFor!.ToListBySeparator("^^SEPARATOR^^")));
            CreateMap<CoursePreviewVideo, CoursePreviewVideoDto>().ReverseMap();
            CreateMap<Picture, PictureDto>().ReverseMap();
            CreateMap<Section, SectionDto>().ReverseMap();
            CreateMap<Lecture, LectureDto>().ReverseMap();
            CreateMap<LectureVideo, LectureVideoDto>().ReverseMap();
            CreateMap<DownloadableFile, DownloadableFileDto>()
                .ForMember(dest => dest.FileUrlPath, options => options.MapFrom<FileUrlResolver>())
                .ReverseMap();
            CreateMap<Subtitle, SubtitleDto>().ReverseMap();
            CreateMap<Quiz, QuizDto>().ReverseMap();
            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<Answer, AnswerDto>().ReverseMap();
            CreateMap<SubCategory, SubCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Rating, RatingCreateDto>().ReverseMap();
            CreateMap<ApplicationUser, InstructorDto>()
                .ForMember(dest => dest.Name, options => options
                    .MapFrom(src => src.FirstName + " " + src.LastName));
            CreateMap<SectionDraftCreateDto, Section>();
            CreateMap<LectureDraftCreateDto, Lecture>();
            CreateMap<QuizDraftCreateDto, Quiz>();
            CreateMap<QuestionDraftCreateDto, Question>()
                .ForMember(dest => dest.Content, options => options
                    .MapFrom(src => src.Question));
            CreateMap<AnswerDraftDto, Answer>()
                .ForMember(dest => dest.Content, options => options
                    .MapFrom(src => src.Answer));
            CreateMap<SectionDraftUpdateDto, Section>();
            CreateMap<LectureDraftUpdateDto, Lecture>();
            CreateMap<QuizDraftUpdateDto, Quiz>();
            CreateMap<QuestionDraftUpdateDto, Question>()
                .ForMember(dest => dest.Content, options => options
                    .MapFrom(src => src.Question));
            CreateMap<RatingUpdateDto, Rating>();
        }
    }
}
