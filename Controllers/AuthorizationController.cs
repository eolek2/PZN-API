using API.Types;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTO;
using API.Enumerations;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    public class AuthorizationController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _autoMapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthorizationController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper autoMapper, IConfiguration config)
        {
            _autoMapper = autoMapper;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToCreate = _autoMapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            var userToReturn = _autoMapper.Map<UserForDetailDto>(userToCreate);

            if (result.Succeeded)
            {
                var createdUser = await _userManager.FindByNameAsync(userToCreate.UserName);
                //Dodanie nowo tworzonym uzytkownikom roli user
                await _userManager.AddToRoleAsync(createdUser, enUserRoles.User.ToString());
                return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(userForLoginDto.UserName);

            if (user == null || !user.CanLogIn || !user.Active)
                return Unauthorized("Wprowadzono niepoprawne dane logowania.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (!signInResult.Succeeded)
                return Unauthorized();

            var token = await GenerateJwtToken(user);

            return Ok(new { token = token, userName = user.UserName, fullName = user.FullName, firstName = user.FirstName, lastName = user.LastName, id = user.Id });
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var expirationDate = DateTime.Now.AddDays(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationDate,
                SigningCredentials = credentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}