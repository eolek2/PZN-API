using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Helpers
{
    public interface IEmailSender
    {
         Task<Response> Send(string fromName, string fromEmail, string subject, string message);
    }
}