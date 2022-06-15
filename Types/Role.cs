using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    // Rola
    public class Role : IdentityRole<int>
    {
        // Kolekcja ról użytkownika
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}