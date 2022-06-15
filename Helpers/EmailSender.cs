using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Helpers
{
    public class EmailSender : IEmailSender
    {
        SendGridOptions _sendGrid;
        public EmailSender(IOptions<SendGridOptions> sendGridOptions)
        {
            _sendGrid = sendGridOptions.Value;
        }

        public async Task<Response> Send(string fromName, string fromEmail, string subject, string message)
         {
             subject = $"[PZN APP]: {subject}";
             var client = new SendGridClient(_sendGrid.SendGridKey);

            var from = new EmailAddress("eolek2@outlook.com", "eolek2");
            var replyTo = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress("eolek2@gmail.com", "Aleksander Ekiert");

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message,
                ReplyTo = replyTo,
            };

            msg.AddTo(to);

            return await client.SendEmailAsync(msg);
         }
    }
}