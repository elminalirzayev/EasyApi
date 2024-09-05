using Domain.Services.Mail;

namespace Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
