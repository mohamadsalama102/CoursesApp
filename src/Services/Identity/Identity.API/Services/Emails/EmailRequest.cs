using MimeKit;

namespace nagiashraf.CoursesApp.Services.Identity.API.Services.Emails;

public class EmailRequest
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile> Attachments { get; set; }
    public EmailRequest(IEnumerable<string> to, string subject, string body)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress(x, x)));
        Subject = subject;
        Body = body;
        Attachments = new List<IFormFile>();
    }

}
