using Microsoft.AspNetCore.Identity;

namespace API.Types
{
    // Rola użytkownika
    public class UserRole : IdentityUserRole<int>
    {
        // Użytkownik
        public virtual User User { get; set; }

        // Rola
        public virtual Role Role { get; set; }
    }
}