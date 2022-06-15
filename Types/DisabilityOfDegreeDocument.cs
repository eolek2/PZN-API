using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class DisabilityOfDegreeDocument
    {
        [Key]
        public int Id {get;set;}
        public int OwnerId {get;set;}
        public virtual User Owner {get;set;}
        public int DisabilityDegreeTypeId {get;set;}
        public virtual DisabilityDegreeType Type {get;set;}
        public virtual ICollection<IdentificationDocumentSymbols> Symbols {get;set;}
        public DateTime ValidFrom {get;set;}
        public DateTime? ValidTo {get;set;}
        public bool ValidAlways {get;set;}
    }
}