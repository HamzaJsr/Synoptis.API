using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;
using Synoptis.API.Models;
using Synoptis.API.Services;
using Synoptis.API.Services.Interfaces;

namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class AppelOffreController : ControllerBase
    {
        private readonly IAppelOffreService _appelOffreService;

        public AppelOffreController(IAppelOffreService appelOffreService)
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

        [HttpGet("{id}", Name = "GetAppelOffreByIdAsync")]
        public async Task<ActionResult<AppelOffreResponseDTO>> GetAppelOffreByIdAsync(Guid id)
        {
            var appelOffre = await _appelOffreService.GetAppelOffreByIdAsync(id);

            if (appelOffre is null)
            {
                return NotFound(new { message = "Pas d'appel d'offre trouvé avec cet id" });
            }

            return Ok(appelOffre);

        }



        [HttpPost("{userId}")]
        public async Task<ActionResult<AppelOffreResponseDTO>> CreateAppelOffreAsync(Guid userId, AppelOffreCreateDTO dto)
        {
            var newAppelOffre = await _appelOffreService.CreateAppelOffreAsync(userId, dto);

            if (newAppelOffre.Id == Guid.Empty) // sécurité en plus
                return BadRequest("ID manquant dans la réponse");



            return CreatedAtAction("GetAppelOffreById", new { id = newAppelOffre.Id }, newAppelOffre);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<AppelOffreResponseDTO>> UpdateAppelOffre(Guid id, [FromBody] AppelOffreUpdateDTO dto)
        {
            var updatedAppelOffre = await _appelOffreService.UpdateAppelOffre(id, dto);

            if (updatedAppelOffre is null)
            {
                return NotFound(new { message = "Pas d'appel d'offre trouvé avec cet id" });
            }

            return Ok(new
            {
                message = $"L'appel d'offre : {updatedAppelOffre.Titre} a bien été modifié",
                data = updatedAppelOffre
            });

        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<AppelOffreResponseDTO>> DeleteAppelOffreAsync(Guid id)
        {
            var appelOffreToDelete = await _appelOffreService.DeleteAppelOffreAsync(id);


            if (appelOffreToDelete is null)
            {
                return NotFound(new { message = "Pas d'appel d'offre trouvé avec cet id" });
            }

            return Ok(new
            {
                message = $"L'appel d'offre : {appelOffreToDelete.Titre} a bien été supprimé",
                data = appelOffreToDelete
            });

        }
    }
}