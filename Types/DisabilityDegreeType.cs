using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class DisabilityDegreeType
    {
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
    }
}