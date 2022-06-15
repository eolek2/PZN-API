using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class MailSendDto
    {
        // Od kogo
        [Required]
        public string FromName {get;set;}
        // Email
        [Required]
        public string FromEmail {get;set;}
        // Temat
        [Required]
        public string Subject {get;set;}
        // Treść
        [Required]
        public string Message {get;set;}
    }
}