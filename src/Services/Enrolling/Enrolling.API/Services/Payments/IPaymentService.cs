using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Services.Payments;

public interface IPaymentService
{
    Task<Enrollment> CreateOrUpdateEnrollmentWithPaymentIntentAsync(string userId, int courseId, decimal coursePrice);
}
