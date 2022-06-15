using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RolesToUpdateDto
    {
        [Required]
        public List<string> Roles {get;set;}
    }
}