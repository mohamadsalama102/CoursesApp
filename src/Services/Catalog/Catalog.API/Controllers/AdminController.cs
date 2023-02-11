using Microsoft.AspNetCore.Mvc;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using System.Net;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Controllers;

[Authorize(Roles = "Moderator")]
[Route("api/v1/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public AdminController(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    [HttpGet("courses-under-review")]
    [ProducesResponseType(typeof(ICollection<CourseDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ICollection<CourseDto>>> GetCoursesUndeerReview()
    {
        var courses = await _courseRepository.GetCoursesUnderReviewAsync();
        return _mapper.Map<List<CourseDto>>(courses);
    }

    [HttpPost("publish-course/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> PublishCourse(int courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);

        if (course == null)
        {
            return NotFound();
        }

        await _courseRepository.PublishCourseAsync(course);

        return NoContent();
    }

    [HttpPost("reject-course/{courseId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RejectCourse(int courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);

        if (course == null)
        {
            return NotFound();
        }

        await _courseRepository.RejectCourseAsync(course);

        return NoContent();
    }
}
