using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;
using Synoptis.API.Services.Interfaces;

namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<ActionResult<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            // .ANY renvoi true si la list contient au moins un elements donc si elle est vide ca renvoie false donc !false ca faity true et on rentre dans le if
            if (!users.Any())
            {
                return Ok(new { message = "La liste des appels d'offres est vide" });
            }

            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserAsync(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);

            if (user is null)
                return NotFound("Pas d'utulisateur trouvé avec cet id");

            if (user.Id == Guid.Empty) // sécurité en plus
                return BadRequest("ID manquant dans la réponse");

            return Ok(user);
        }
    }
}