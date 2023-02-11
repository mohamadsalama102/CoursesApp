using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentContext _context;

    public EnrollmentRepository(EnrollmentContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
    {
        return await _context.Enrollments.SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment?> GetUnsuccessfullEnrollmentForUserAsync(string userId)
    {
        return await _context.Enrollments.SingleOrDefaultAsync(e => e.StudentId == userId && !e.PaymentSucceeded
            && e.PaymentIntentId != null);
    }

    public async Task<Enrollment?> GetEnrollmentByPaymentIntentIdAsync(string paymentInentId)
    {
        return await _context.Enrollments.SingleOrDefaultAsync(e => e.PaymentIntentId == paymentInentId);
    }

    public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return enrollment;
    }

    public async Task<bool> CheckIfSuccessfulEnrollmentExistsAsync(string studentId, int courseId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => (e.StudentId == studentId && e.CourseId == courseId && e.PaymentSucceeded)
                || (e.StudentId == studentId && e.CourseId == courseId && e.CoursePrice == 0));
    }

    public async Task SetTruePaymentSucceededAsync(Enrollment enrollment)
    {
        enrollment.PaymentSucceeded = true;
        await _context.SaveChangesAsync();
    }
}
