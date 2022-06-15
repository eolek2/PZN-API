using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class IdentificationDocument
    {
        [Key]
        public int Id {get;set;}
        
        public int IdentificationDocumentTypeId {get;set;}
        public virtual IdentificationDocumentType Type {get;set;}
        public virtual User User {get;set;}
        public int UserId {get;set;}
        public string Number {get;set;}
        public DateTime ValidFrom {get;set;}
        public DateTime? ValidTo {get;set;}
        public bool ValidAlways {get;set;}
    }
}