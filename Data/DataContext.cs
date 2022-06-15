using API.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>
    , IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> Posts {get;set;}
        public DbSet<IdentificationDocumentType> IdentificationDocumentTypes {get;set;}
        public DbSet<DisabilityDegreeType> DisabilityDegreeTypes {get;set;}
        public DbSet<DisabilityDegreeDict> DisabilityDegreeSymbols {get;set;}
        public DbSet<DisabilityOfDegreeDocument> DisabilityOfDegreeDocuments {get;set;}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

                 userRole.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });
            
            builder.Entity<User>()
                .Property(u => u.Active)
                .HasDefaultValue(true);
        }
    }
}