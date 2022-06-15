using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class PostForUpdateDto
    {
        // Tytuł posta
        [Required]
        [MinLength(3)]
        public string Title {get;set;}
        // Krótka treść
        [Required]
        [MinLength(15)]
        public string ShortContent {get;set;}
        // Treść
        [Required]
        [MinLength(15)]
        public string Content {get;set;}
    }
}