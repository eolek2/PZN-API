using API.Enumerations;
using API.Types;
using Bogus;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    // Klasa seedująca dane
    public class Seed
    {
        #region Dane domyślne
        internal static async void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager, DataContext context, Faker<User> userFaker)
        {
            var roles = new List<Role> { };
            foreach (var role in Enum.GetNames(typeof(enUserRoles)))
            {
                roles.Add(new Role { Name = role });
            }

            //Dodaje role jesli nie ma
            foreach (Role role in roles)
            {
                if (!roleManager.Roles.Any(a => a.Name == role.Name))
                {
                    roleManager.CreateAsync(role).Wait();
                }
            }

            if (!userManager.Users.Any())
            {
                AddAdminUser(userManager, roleManager);
            }

            AddDisabilityDegreeTypes(context);
            AddIdentificationDocumentTypes(context);
            AddDisabilityDegreeDictionary(context);

            if(context.Users.Count() < 10)
            {
for(int i = 0; i<1000;i++)
            {
                var user = userFaker.Generate();

                if(!context.Users.Any(x => x.UserName == user.UserName))
                {
                    var created = userManager.CreateAsync(user, "Pa$$word1").Result;

                    if(created.Succeeded)
                    {
                        var createdUser = userManager.FindByNameAsync(user.UserName).Result;

                        if(createdUser != null)
                        {
                            userManager.AddToRolesAsync(createdUser, new[] { enUserRoles.User.ToString() }).Wait();
                        }
                    }
                }
            }
            }
        }
        
        private static void AddAdminUser(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var admin = new User
            {
                UserName = "admin",
                FirstName = "Administrator",
                LastName = "Systemu",
                Email = "eolek2@gmail.com",
                CanLogIn = true,
                Active = true,
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(admin, "Pa$$word1").Result;
            if (result.Succeeded)
            {
                var createdUser = userManager.FindByNameAsync("admin").Result;
                userManager.AddToRolesAsync(createdUser, new[] { "Administrator" }).Wait();
            }
        }

        private static void AddIdentificationDocumentTypes(DataContext context)
        {
            List<string> types = new List<string>();
            types.AddRange(new [] 
            {
                "Dowód osobisty",
                "Paszport"
            }
            );

            foreach(var type in types)
            {
                if(!context.IdentificationDocumentTypes.Any(x=>x.Name == type))
                    context.IdentificationDocumentTypes.Add(new IdentificationDocumentType { Name = type });
            }

            context.SaveChanges();
        }

        private static void AddDisabilityDegreeTypes(DataContext context)
        {
            List<string> types = new List<string>();
            types.AddRange(new [] 
            {
                "Lekki",
                "Umiarkowany",
                "Znaczny"
            }
            );

            foreach(var type in types)
            {
                if(!context.DisabilityDegreeTypes.Any(x=>x.Name == type))
                    context.DisabilityDegreeTypes.Add(new DisabilityDegreeType { Name = type });
            }

            context.SaveChanges();
        }

        private static void AddDisabilityDegreeDictionary(DataContext context)
        {
            List<DisabilityDegreeDict> types = new List<DisabilityDegreeDict>();
            types.AddRange(new DisabilityDegreeDict[] 
            {
                new DisabilityDegreeDict("01-U","Upośledzenie umysłowe"),
                new DisabilityDegreeDict("02-P","Choroby psychiczne"),
                new DisabilityDegreeDict("03-L","Zaburzenia głosu, mowy i choroby słuchu"),
                new DisabilityDegreeDict("04-O","Choroby narządu wzroku"),
                new DisabilityDegreeDict("05-R","Upośledzenie narządu ruchu"),
                new DisabilityDegreeDict("06-E","Epilepsja"),
                new DisabilityDegreeDict("07-S","Choroby układu oddechowego i krążenia"),
                new DisabilityDegreeDict("08-T","Choroby układu pokarmowego"),
                new DisabilityDegreeDict("09-M","Choroby układu moczowo-płciowego"),
                new DisabilityDegreeDict("10-N","Choroby neurologiczne"),
                new DisabilityDegreeDict("11-I","Inne, w tym schorzenia: endokrynologiczne, metaboliczne, zaburzenia enzymatyczne, choroby zakaźne i odzwierzęce, zeszpecenia, choroby układu krwiotwórczego"),
                new DisabilityDegreeDict("12-C","Całościowe zaburzenia rozwojowe"),
            }
            );

            foreach(var type in types)
            {
                if(!context.DisabilityDegreeSymbols.Any(x=>x.Symbol == type.Symbol))
                    context.DisabilityDegreeSymbols.Add(type);
            }

            context.SaveChanges();
        }

        #endregion
    }
}