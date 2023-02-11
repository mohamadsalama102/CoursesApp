using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetEnrollmentByIdAsync(int id);
    Task<Enrollment?> GetUnsuccessfullEnrollmentForUserAsync(string userId);
    Task<Enrollment?> GetEnrollmentByPaymentIntentIdAsync(string paymentInentId);
    Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
    Task<bool> CheckIfSuccessfulEnrollmentExistsAsync(string studentId, int courseId);
    Task SetTruePaymentSucceededAsync(Enrollment enrollment);
}
