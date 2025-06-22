using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;
using Synoptis.API.Services.Interfaces;

namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserRegisterResponseDTO>> RegisterAsync(UserRegisterDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDTO>> LoginAsync(LoginDTO dto)
        {
            var result = await _userService.LoginAsync(dto);

            if (result.Token is null)
                return Unauthorized(result.ErrorMessage ?? "Erreur d'authentification");

            return Ok(new { token = result.Token });
        }

        [Authorize(Roles = "ResponsableAgence")]
        [HttpGet]
        public ActionResult RouteTestAuthorisation()
        {
            return Ok(new { message = "Tu est bien un responsable" });
        }
    }
}