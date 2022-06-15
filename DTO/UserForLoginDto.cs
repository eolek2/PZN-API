using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class UserForLoginDto
    {
        // Login
        [Required]
        public string UserName {get;set;}
        // Hasło
        [Required]
        public string Password {get;set;}
    }
}