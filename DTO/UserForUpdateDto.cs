using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class UserForUpdateDto
    {
        [Required]
        public string FirstName {get;set;}
        
        [Required]
        public string LastName {get;set;}

        [Required]
        public DateTime? DateOfBirth {get;set;}

        [Required]
        public string CityOfBirth {get;set;}

        [Required]
        public string PESEL {get;set;}

        [Required]
        public string PhoneNumber {get;set;}
    }
}