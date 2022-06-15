namespace API.DTO
{
    public class UserForListDto
    {
        public int UserId {get;set;}
        public string UserName {get;set;}
        public string FullName {get;set;}
        public string Email {get;set;}
        public bool HasAvatar {get;set;}
        public string AvatarUrl {get;set;}
    }
}