namespace API.DTO
{
    public class UserForDetailDto
    {
        public int UserId {get;set;}
        public string UserName {get;set;}
        public string FullName {get;set;}
        public string Email {get;set;}
        public bool CanWritePosts {get;set;}
        public bool HasAvatar {get;set;}
        public string AvatarUrl {get;set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public DateTime DateOfBirth {get;set;}
        public string CityOfBirth {get;set;}
        public string PESEL {get;set;}
        public string PhoneNumber {get;set;}
        public string DateOfBirthText
        {
            get
            {
                return DateOfBirth.ToString("dd MMM yyyy");
            }
        }

        public int Age
        {
            get
            {
                int ret = (DateTime.Now.Year - DateOfBirth.Year);
                if(DateTime.Now.Month < DateOfBirth.Month)
                    ret--;
                else if(DateTime.Now.Month == DateOfBirth.Month && DateTime.Now.Day < DateOfBirth.Day)
                    ret --;

                return ret;
            }
        }
    }
}