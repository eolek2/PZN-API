using System.ComponentModel.DataAnnotations;

namespace API.Types
{
    public class BlogPost
    {
        [Key]
        public int Id {get;set;}
        public DateTime CreationDate {get;set;}
        public int UserId {get;set;}
        public virtual User CreatedBy {get;set;}
        public DateTime? UpdateDate {get;set;}
        public string Title {get;set;}
        public string ShortContent {get;set;}
        public string Content {get;set;}
        public string ImageUrl {get;set;} = "";
    }
}