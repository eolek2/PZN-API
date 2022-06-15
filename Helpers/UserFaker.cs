using API.DTO;
using API.Types;
using AutoMapper;
using Bogus;

namespace API.Helpers
{
    public class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            StrictMode(false);
            RuleFor(p => p.Active, f => true);
            RuleFor(p => p.AvatarUrl, f => f.Person.Avatar);
            RuleFor(p => p.CanLogIn, f => true);
            RuleFor(p => p.CityOfBirth, f => f.Address.City());
            RuleFor(p => p.DateOfBirth, f => f.Person.DateOfBirth);
            RuleFor(p => p.Email, f => f.Person.Email);
            RuleFor(p => p.EmailConfirmed, true);
            RuleFor(p => p.FirstName, f => f.Person.FirstName);
            RuleFor(p => p.LastName, f => f.Person.LastName);
            RuleFor(p => p.PESEL, f => "11111111111");
            RuleFor(p => p.PhoneNumber, f => f.Person.Phone);
            RuleFor(p => p.PhoneNumberConfirmed, true);
            RuleFor(p => p.UserName, f => f.Person.UserName);
            RuleFor(p => p.UserRoles, f => new List<UserRole>());
            RuleFor(p => p.Documents, f => new List<IdentificationDocument>());
            RuleFor(p => p.Id, f => 0);
            RuleFor(p => p.NormalizedUserName, f => String.Empty);
            RuleFor(p => p.NormalizedEmail, f => String.Empty);
            RuleFor(p => p.PasswordHash, f => String.Empty);
            RuleFor(p => p.SecurityStamp, f => String.Empty);
            RuleFor(p => p.ConcurrencyStamp, f => String.Empty);
            RuleFor(p => p.TwoFactorEnabled, f => false);
            RuleFor(p => p.LockoutEnd, f => null);
            RuleFor(p => p.LockoutEnabled, f => false);
            RuleFor(p => p.AccessFailedCount, f => 0);
        }
    }
}