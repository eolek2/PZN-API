using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class IdentificationDocumentSymbols
    {
        [Key]
        public int Id {get;set;}
        
        public int DisabilityOfDegreeDocumentId {get;set;}
        public virtual DisabilityOfDegreeDocument Document {get;set;}
        public virtual DisabilityDegreeDict Symbol {get;set;}
        public int DisabilityDegreeDictId {get;set;}
    }
}