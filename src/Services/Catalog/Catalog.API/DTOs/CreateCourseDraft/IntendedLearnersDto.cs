using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class IntendedLearnersDto
{
    [MaxLengthForIEnumerableStringElement(160)]
    public List<string>? WhatYouWillLearn { get; set; }

    public List<string>? Requirements { get; set; }

    public List<string>? WhoIsThisCourseFor { get; set; }

    [Required]
    public int CourseId { get; set; }
}
