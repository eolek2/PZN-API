namespace API.DTO
{
    public class PostForDetailDto
    {
        public int Id {get;set;}
        public DateTime CreationDate {get;set;}
        public string CreatedBy {get;set;}
        public int CreatedById {get;set;}
        public DateTime? UpdateDate {get;set;}
        public bool WasUpdated {get;set;}
        public string Title {get;set;}
        public string Content {get;set;}
        public string ImageUrl {get;set;}
        public bool HasImage {get;set;}
        public bool HasNext {get;set;}
        public bool HasPrevious {get;set;}
        public int? NextId {get;set;}
        public int? PreviousId {get;set;}
    }
}