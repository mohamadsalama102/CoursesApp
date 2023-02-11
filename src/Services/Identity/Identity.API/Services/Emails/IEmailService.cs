namespace nagiashraf.CoursesApp.Services.Identity.API.Services.Emails;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest mailRequest);
}
