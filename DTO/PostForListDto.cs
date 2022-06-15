namespace API.DTO
{
    public class PostForListDto
    {
        public int Id {get;set;}
        public DateTime CreationDate {get;set;}
        public string CreatedBy {get;set;}
        public int CreatedById {get;set;}
        public DateTime? UpdateDate {get;set;}
        public bool WasUpdated {get;set;}
        public string Title {get;set;}
        public string Content {get;set;}
    }
}