using API.DTO;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class MailController : BaseController
    {
        private readonly IEmailSender _sendgrid;
        public MailController(IEmailSender emailSender)
        {
            _sendgrid = emailSender;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendEmail(MailSendDto mailData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _sendgrid.Send(mailData.FromName, mailData.FromEmail, mailData.Subject, mailData.Message);

            if(result.IsSuccessStatusCode)
                return Ok();
            else
                return BadRequest("Nie udało się wysłać wiadomości E-mail");
        }
    }
}