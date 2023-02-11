using Microsoft.Extensions.Options;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;
using Stripe;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public PaymentService(IOptions<StripeSettings> stripeSettings, IEnrollmentRepository enrollmentRepository)
    {
        _stripeSettings = stripeSettings.Value;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Enrollment> CreateOrUpdateEnrollmentWithPaymentIntentAsync(string userId, int courseId, decimal coursePrice)
    {
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        var paymentIntentService = new PaymentIntentService();

        var enrollment = await _enrollmentRepository.GetUnsuccessfullEnrollmentForUserAsync(userId);

        if (enrollment == null)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)coursePrice * 100,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            var intent = await paymentIntentService.CreateAsync(options);

            enrollment = await _enrollmentRepository.CreateEnrollmentAsync(new Enrollment
            {
                StudentId = userId,
                CourseId = courseId,
                CoursePrice = coursePrice,
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret
            });
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)coursePrice * 100,
            };
            await paymentIntentService.UpdateAsync(enrollment.PaymentIntentId, options);
        }

        return enrollment;
    }
}
