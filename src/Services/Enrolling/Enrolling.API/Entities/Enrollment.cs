namespace nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

public class Enrollment
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public decimal CoursePrice { get; set; }
    public bool PaymentSucceeded { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
}
