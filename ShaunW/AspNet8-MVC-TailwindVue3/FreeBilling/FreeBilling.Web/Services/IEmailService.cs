namespace FreeBilling.Web.Services;

public interface IEmailService
{
    void SendMail(string from, string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> logger;

    public EmailService(ILogger<EmailService> logger)
    {
        this.logger = logger;
    }

    public void SendMail(string from, string to, string subject, string body)
    {
        logger.LogInformation($"Sending email to {to}, from: {from}, subject: {subject}, body: {body.Substring(0, 10)}...");
    }
}
