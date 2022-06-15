using API.Data;
using API.DTO;
using API.Filters;
using API.Types;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DegreeOfDisabilityController : BaseController
    {
        IDataRepository _repository;
        UserManager<User> _userManager;
        IMapper _autoMapper;
        public DegreeOfDisabilityController(IDataRepository repository, UserManager<User> userManager, IMapper autoMapper)
        {
            _repository = repository;
            _userManager = userManager;
            _autoMapper = autoMapper;
        }

        [HttpPost]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> AddDisabilityDegree(DisabilityDegreeToAddDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);

            return BadRequest("Nie udało się dodać orzeczenia.");
        }
    }
}