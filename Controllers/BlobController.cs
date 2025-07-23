
using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;

namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlobController : ControllerBase
    {
        private readonly BlobStorageService _blobService;

        public BlobController(BlobStorageService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> List()
        {
            var files = await _blobService.ListFilesAsync();
            return Ok(files); // renvoie un tableau JSON de noms de fichiers
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Fichier invalide");

            await _blobService.UploadAsync(request.File, request.ClientId, request.Category);
            return Ok("Fichier envoyé avec succès");
        }

    }

}


