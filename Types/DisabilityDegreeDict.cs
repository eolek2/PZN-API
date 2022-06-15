using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    public class DisabilityDegreeDict
    {
        public DisabilityDegreeDict()
        {
            
        }

        public DisabilityDegreeDict(string symbol, string description)
        {
            Symbol = symbol; Description = description;
        }

        [Key]
        public int Id {get;set;}
        public string Symbol {get;set;}
        public string Description {get;set;}
    }
}