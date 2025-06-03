using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;
using Synoptis.API.Models;
using Synoptis.API.Services;

namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppelOffreController : ControllerBase
    {
        private readonly AppelOffreService _appelOffreService;

        public AppelOffreController(AppelOffreService appelOffreService)
        {
            _appelOffreService = appelOffreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppelOffre>>> GetAllAppelOffresAsync()
        {
            var appelOffres = await _appelOffreService.GetAllAppelOffresAsync();
            // .ANY renvoi true si la list contient au moins un elements donc si elle est vide ca renvoie false donc !false ca faity true et on rentre dans le if
            if (!appelOffres.Any())
            {
                return Ok(new { message = "La liste des appels d'offres est vide" });
            }

            return Ok(appelOffres);
        }

        [HttpGet("{id}", Name = "GetAppelOffreById")]
        public async Task<ActionResult<AppelOffreResponseDTO>> GetAppelOffreByIdAsync(Guid id)
        {
            var appelOffre = await _appelOffreService.GetAppelOffreByIdAsync(id);

            if (appelOffre is null)
            {
                return NotFound(new { message = "Pas d'appel d'offre trouvé avec cet id" });
            }

            return Ok(appelOffre);

        }



        [HttpPost]
        public async Task<ActionResult<AppelOffreResponseDTO>> CreateAppelOffreAsync(AppelOffreCreateDTO dto)
        {
            var newAppelOffre = await _appelOffreService.CreateAppelOffreAsync(dto);

            if (newAppelOffre.Id == Guid.Empty) // sécurité en plus
                return BadRequest("ID manquant dans la réponse");
            Console.WriteLine($"ID généré : {newAppelOffre.Id}");


            return CreatedAtAction("GetAppelOffreById", new { id = newAppelOffre.Id }, newAppelOffre);

        }
    }
}