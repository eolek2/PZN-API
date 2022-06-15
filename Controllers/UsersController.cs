using API.Data;
using API.DTO;
using API.Enumerations;
using API.Filters;
using API.Helpers;
using API.Types;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _autoMapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _sendgrid;

        public UsersController(IDataRepository dataRepository, IMapper autoMapper, UserManager<User> userManager, SignInManager<User> signInManager
                            , IEmailSender sendgrid)
        {
            _autoMapper = autoMapper;
            _dataRepository = dataRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendgrid = sendgrid;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilters filters, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userToReturn = await _dataRepository.GetUsers(filters, cancellationToken);
            var usersForDetailedDto = _autoMapper.Map<ICollection<UserForListDto>>(userToReturn);
            return Ok(usersForDetailedDto);
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("{id}",Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userMakingReq = await _userManager.GetUserAsync(User);
            User userToReturn = null;
            if (await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Administrator.ToString()) 
            || await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Moderator.ToString()))
            {
                userToReturn = await _dataRepository.GetUser(id, cancellationToken);
            }
            else if(await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.User.ToString()))
            {
                if(id != userMakingReq.Id)
                {
                    userToReturn = await _dataRepository.GetUser(id, cancellationToken);

                    userToReturn.PESEL = String.Empty;
                }
                else
                    userToReturn = userMakingReq;
            }

            if(userToReturn == null)
                return NotFound(id);

            var userForDetailedDto = _autoMapper.Map<UserForDetailDto>(userToReturn);

            if (await _userManager.IsInRoleAsync(userToReturn, enUserRoles.Administrator.ToString()) 
            || await _userManager.IsInRoleAsync(userToReturn, enUserRoles.Moderator.ToString()))
            {
                userForDetailedDto.CanWritePosts = true;
            }

            return Ok(userForDetailedDto);
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPatch("{id}/UpdateRoles")]
        public async Task<IActionResult> UpdateRoles(int id, RolesToUpdateDto roles, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if (await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Administrator.ToString()) 
            || await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Moderator.ToString()))
            {
                var user = await _dataRepository.GetUser(id, cancellationToken);
                foreach(var role in roles.Roles)
                {
                    if(!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                var existingRoles = await _userManager.GetRolesAsync(user);
                if(existingRoles != null)
                {
                    foreach(var role in existingRoles)
                    {
                        if(!roles.Roles.Contains(role))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                    }
                }

                return Ok();
            }

            return Unauthorized();
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPatch("{id}/ActivateAccount")]
        public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if (await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Administrator.ToString()) 
            || await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Moderator.ToString()))
            {
                var user = await _dataRepository.GetUser(id, cancellationToken);
                user.Active = true;
                user.CanLogIn = true;

                await _userManager.UpdateAsync(user);

                return Ok();
            }

            return Unauthorized();
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPatch("{id}/DectivateAccount")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if (await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Administrator.ToString()) 
            || await _userManager.IsInRoleAsync(userMakingReq, enUserRoles.Moderator.ToString()))
            {
                var user = await _dataRepository.GetUser(id, cancellationToken);
                user.Active = false;
                user.CanLogIn = false;

                await _userManager.UpdateAsync(user);

                return Ok();
            }

            return Unauthorized();
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPatch("{id}/AddAvatar")]
        public async Task<IActionResult> AddAvatar(int id, AvatarForAddDto avatarToAdd, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if(userMakingReq.Id != id)
                return Unauthorized();

            var user = await _dataRepository.GetUser(userMakingReq.Id, cancellationToken);
            user.AvatarUrl = avatarToAdd.Url;
            await _userManager.UpdateAsync(user);

            return Ok();
        } 

        [Authorize(Policy = "RequireUserRole")]
        [HttpPatch("{id}/RemoveAvatar")]
        public async Task<IActionResult> RemoveAvatar(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if(userMakingReq.Id != id)
                return Unauthorized();

            var user = await _dataRepository.GetUser(userMakingReq.Id, cancellationToken);
            user.AvatarUrl = "";
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);

            if(userMakingReq.Id != id)
                return Unauthorized();

            var user = await _dataRepository.GetUser(userMakingReq.Id, cancellationToken);

            if(user == null)
                return BadRequest("Nie udało się pobrać danych użytkownika");

            user.CityOfBirth = dto.CityOfBirth;
            user.DateOfBirth = dto.DateOfBirth;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PESEL = dto.PESEL;
            user.PhoneNumber = dto.PhoneNumber;

            await _userManager.UpdateAsync(user);

            var userToReturn = _autoMapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPatch("{id}/ChangePassword")]
        public async Task<IActionResult> ChangePassword(int id, PasswordForChangeDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            
            if(userMakingReq.Id != id)
                return Unauthorized();

            var user = await _dataRepository.GetUser(userMakingReq.Id, cancellationToken);

            if (user == null || !user.CanLogIn || !user.Active)
                return Unauthorized("Wprowadzono niepoprawne dane logowania.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.OldPassword, false);

            if (!signInResult.Succeeded)
                return Unauthorized();
            
            await _userManager.ChangePasswordAsync(user,dto.OldPassword, dto.NewPassword);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(UserForResetPasswordDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _dataRepository.FindUserByEmail(dto.Email, cancellationToken);

            if (user == null || !user.CanLogIn || !user.Active)
                return Unauthorized();

            char[] specialChars = {'!','@','#','$','%','^','&','*','(',')','_','-','+','='};
            char[] letters = {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
            char[] numbers = {'1','2','3','4','5','6','7','8','9','0'};
            var newPassword = "";

            Random r = new Random();
            int scl = specialChars.Count() -1;
            int ll = letters.Count() - 1;
            int nl = numbers.Count() -1;
            for(int i = 0; i<16;i++)
            {
                if(i%4 == 0)
                {
                    int iscl = r.Next(0, scl);
                    newPassword += $"{specialChars[iscl]}";
                }
                else if(i%3 == 0)
                {
                    int inl = r.Next(0, nl);
                    newPassword += $"{numbers[inl]}";
                }
                else if(i%2 == 0)
                {
                    int sl = r.Next(0, ll);
                    newPassword += $"{letters[sl]}";
                }
                else
                {
                    int bl = r.Next(0, ll);
                    newPassword += $"{letters[bl].ToString().ToUpper()}";
                }
            }

            await _userManager.RemovePasswordAsync(user);

            await _userManager.AddPasswordAsync(user, newPassword);

            var message = $@"
            Twoje nowe hasło do serwisu to: <b>{newPassword}</b>
            ";

            await _sendgrid.Send("PZN APP", "eolek2@outlook.com", "Nowe hasło do systemu", message);

            return Ok();
        }
    }
}