using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class UserForResetPasswordDto
    {
        [Required]
        public string Email {get;set;}
    }
}