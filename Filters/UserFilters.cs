namespace API.Filters
{
    public class UserFilters
    {
        public int? UserId {get;set;}
        public DateTime? FromDate {get;set;}
        public DateTime? ToDate {get;set;}
        public int? Age {get;set;}
        public string UserName {get;set;} = "";
        public string Email {get;set;} = "";
        public string FirstName {get;set;} = "";
        public string LastName {get;set;} = "";
    }
}