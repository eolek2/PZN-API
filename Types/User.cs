using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    // Definicja atrybutów użytkownika
    public class User : IdentityUser<int>
    {
        // Imię
        public string FirstName {get;set;}
        // Nazwisko
        public string LastName {get;set;}

        public string CityOfBirth {get;set;} = "";

        public string PESEL {get;set;} = "";

        // Data urodzenia
        public DateTime? DateOfBirth {get;set;}

        // Czy użytkownik jest aktywny
        public bool Active { get; set; }

        // Czy użytkownik może się zalogować w aplikacji
        public bool CanLogIn { get; set; }

        // Lista ról użytkownika
        public virtual ICollection<UserRole> UserRoles{ get; set; }

        // Imię i nazwisko
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        // Zdjęcie profilowe
        public string AvatarUrl {get;set;} = "";

        public virtual ICollection<IdentificationDocument> Documents {get;set;}

        // Wiek
        public int? Age
        {
            get
            {
                if(DateOfBirth.HasValue)
                {
                    // Obliczamy różnicę lat rok bieżący - rok urodzenia
                    int years = DateTime.Now.Year - DateOfBirth.Value.Year;

                    // Gdy przed urodzinami, zmniejszamy wiek o 1;
                    if(DateTime.Now.Date < DateOfBirth.Value.Date)
                        years = years - 1;

                    return years;
                }
                
                return null;
            }
        }
    }
}