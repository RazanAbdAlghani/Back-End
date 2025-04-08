using secondVersionFlowSync.DTOs;

namespace secondVersionFlowSync.services.EmailService
{
    public interface IEmailService
    {
        Task sendEmailAsync(EmailDto request);
    }
}
