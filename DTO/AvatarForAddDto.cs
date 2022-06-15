using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class AvatarForAddDto
    {
        // Url avatara
        [Required]
        public string Url {get;set;}
    }
}