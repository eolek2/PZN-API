using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class UserForRegisterDto
    {
        // Login
        [Required]
        public string UserName {get;set;}
        // Email
        [Required]
        public string Email {get;set;}
        // Hasło
        [Required]
        public string Password {get;set;}
        [Required]
        public string FirstName {get;set;}
        [Required]
        public string LastName {get;set;}
    }
}