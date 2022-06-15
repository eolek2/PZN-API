using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class DisabilityDegreeToAddDto
    {
        // Url skanu orzeczenia
        [Required]
        public string Url {get;set;}
        [Required]
        public int UserId {get;set;}
        [Required]
        public int DocumentId {get;set;}
        [Required]
        public List<string> CodesOfDegree {get;set;}
        [Required]
        public string DegreeOfDisabilityName {get;set;}
        [Required]
        public DateTime FromDate {get;set;}
        [Required]
        public DateTime ToDate {get;set;}
    }
}