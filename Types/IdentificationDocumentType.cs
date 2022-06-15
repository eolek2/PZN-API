using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class IdentificationDocumentType
    {
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
    }
}