using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.DTOs;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents;
using nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents.Events;
using nagiashraf.CoursesApp.Services.Enrolling.API.Services.Payments;
using Stripe;
using System.Net;
using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
public class EnrollmentController : ControllerBase
{
    private const string WebhookSecret = "whsec_c992afd5dab568838582a42195e4de80e4c0c04f6016f529a4279476af08ad22";

    private readonly IPaymentService _paymentService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IEnrollingIntegrationEventService _enrollingIntegrationEventService;
    private readonly ICatalogGrpcClient _catalogGrpcClient;

    public EnrollmentController(IEnrollmentRepository enrollmentRepository, IPaymentService paymentService,
        IEnrollingIntegrationEventService enrollingIntegrationEventService, ICatalogGrpcClient catalogGrpcClient)
    {
        _enrollmentRepository = enrollmentRepository;
        _paymentService = paymentService;
        _enrollingIntegrationEventService = enrollingIntegrationEventService;
        _catalogGrpcClient = catalogGrpcClient;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Enrollment>> GetEnrollmentById(int id)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound("Enrollment not found");
        }

        return enrollment;
    }

    [HttpPost("enroll-free")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> EnrollInFreeCourse([FromBody] int courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return NotFound("User not found");

        bool studentAlreadyEnrolledInCourse = await _enrollmentRepository.CheckIfSuccessfulEnrollmentExistsAsync(userId, courseId);
        if (studentAlreadyEnrolledInCourse)
            return BadRequest("Student is already enrolled in this course.");

        var price = await _catalogGrpcClient.GetCoursePriceAsync(courseId);
        if (price > 0)
            return BadRequest($"Course with Id {courseId} is not free.");

        var enrollment = await _enrollmentRepository.CreateEnrollmentAsync(new Enrollment
        {
            StudentId = userId,
            CourseId = courseId,
            CoursePrice = 0
        });

        var studentEnrolledInCourseEvent = new StudentEnrolledInCourseIntegrationEvent(enrollment.StudentId, enrollment.CourseId);
        _enrollingIntegrationEventService.PublishThroughEventBusAsync(studentEnrolledInCourseEvent);

        return NoContent();
    }

    [HttpPost("payment-intent")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Enrollment), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateOrUpdateEnrollmentWithPaymentIntent(CourseDto courseDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return NotFound($"User with Id {userId} not found");

        bool studentAlreadyEnrolledInCourse = await _enrollmentRepository.CheckIfSuccessfulEnrollmentExistsAsync(userId, courseDto.CourseId);
        if (studentAlreadyEnrolledInCourse)
            return BadRequest("Student is already enrolled in this course.");

        var price = await _catalogGrpcClient.GetCoursePriceAsync(courseDto.CourseId);
        if (price != courseDto.Price)
            return BadRequest("Incorrect course price");

        var enrollment = await _paymentService.CreateOrUpdateEnrollmentWithPaymentIntentAsync(userId, courseDto.CourseId,
            courseDto.Price);

        return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WebhookSecret);

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var stripeReturnedIntent = (PaymentIntent)stripeEvent.Data.Object;
            var enrollment = await _enrollmentRepository.GetEnrollmentByPaymentIntentIdAsync(stripeReturnedIntent.Id);

            if (enrollment == null)
                return BadRequest("Payment intent not found");

            var userId = enrollment.StudentId;
            if (userId == null)
                return BadRequest("User not found");

            await _enrollmentRepository.SetTruePaymentSucceededAsync(enrollment);

            var studentEnrolledInCourseEvent = new StudentEnrolledInCourseIntegrationEvent(enrollment.StudentId, enrollment.CourseId);
            _enrollingIntegrationEventService.PublishThroughEventBusAsync(studentEnrolledInCourseEvent);
        }

        return new EmptyResult();
    }
}
