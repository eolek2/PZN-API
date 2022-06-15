using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class PasswordForChangeDto
    {
        [Required]
        public string OldPassword {get;set;}
        [Required]
        public string NewPassword {get;set;}
    }
}