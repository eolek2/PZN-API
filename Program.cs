using System.Net;
using System.Text;
using API.Data;
using API.Enumerations;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using API.Types;
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( opt => {
    opt.SwaggerDoc("v1", new OpenApiInfo(){
        Title = "PZN API", Version = "v1"
    });
    opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    opt.IgnoreObsoleteActions();
    opt.IgnoreObsoleteProperties();
    opt.CustomSchemaIds(type => type.FullName);
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
});

// Inicjalizacja kontekstu bazy danych (MS SQL Server)
builder.Services.AddDbContextPool<DataContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Definicja zabezpieczeń
var ib = builder.Services.AddIdentityCore<User>(options =>
{
    // Czy hasło ma posiadać litery
    options.Password.RequireDigit = true;
    // Minimalna długość hasła
    options.Password.RequiredLength = 6;
    // Czy hasło ma mieć znak specjalny
    options.Password.RequireNonAlphanumeric = true;
    // Czy hasło ma mieć dużą literę
    options.Password.RequireUppercase = true;
});

var emailTokenProviderType = typeof(EmailTokenProvider<User>);

ib = new IdentityBuilder(ib.UserType, typeof(Role), ib.Services);
ib.AddEntityFrameworkStores<DataContext>();
ib.AddRoleValidator<RoleValidator<Role>>();
ib.AddRoleManager<RoleManager<Role>>();
ib.AddSignInManager<SignInManager<User>>();
ib.AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType);
ib.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(enUserRoles.Administrator.ToString()));
    options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole(enUserRoles.Administrator.ToString(), enUserRoles.Moderator.ToString()));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole(enUserRoles.Administrator.ToString(), enUserRoles.Moderator.ToString(), enUserRoles.User.ToString()));
});

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddCors();

builder.Services.AddAutoMapper(typeof(DataContext).Assembly).AddDbContext<DataContext>();

builder.Services.AddScoped<IDataRepository, DataRepository>();
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGridSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSingleton<Faker<User>, UserFaker>();
var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        services.GetRequiredService<DataContext>().Database.Migrate();
        Seed.SeedUsers(services.GetRequiredService<UserManager<User>>(), services.GetRequiredService<RoleManager<Role>>(), services.GetRequiredService<DataContext>(), services.GetRequiredService<Faker<User>>());         
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Blad podczas migracji");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();