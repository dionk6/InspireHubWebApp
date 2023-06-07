using InspireHubWebApp.DTOs;

namespace InspireHubWebApp.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Application contact);
        Task<bool> SendMessageAsync(string emails, string message);
    }
}
