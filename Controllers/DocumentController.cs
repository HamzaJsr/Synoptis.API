
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synoptis.API.DTOs;


namespace Synoptis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly BlobStorageService _blobService;
        private readonly IHttpContextAccessor _httpContext;

        public DocumentController(BlobStorageService blobService, IHttpContextAccessor httpContext)
        {
            _blobService = blobService;
            _httpContext = httpContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> List()
        {
            var files = await _blobService.ListFilesAsync();
            return Ok(files); // renvoie un tableau JSON de noms de fichiers
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] UploadDocumentRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Fichier invalide.");


            // Récupérer l'utilisateur connecté
            var userId = _httpContext.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();


            var document = await _blobService.UploadDocumentAsync(request, userId);

            return Ok(new { message = "Upload réussi", document });
        }

        [Authorize]
        [HttpDelete("delete/{documentId}")]
        public async Task<ActionResult<AppelOffreDocumentDTO?>> DeleteDocumentAsync(Guid documentId)
        {
            var doc = await _blobService.DeleteDocumentAsync(documentId);

            if (doc == null)
                return NotFound(new { message = "Pas de document trouvé avec cet id" });

            return Ok(new
            {
                message = $"L'appel d'offre : {doc.NomFichier} a bien été supprimé",
                data = doc
            });
        }

    }

}


// public IFormFile File { get; set; } = null!;
//     public Guid AppelOffreId { get; set; }
//     public string TypeDocument { get; set; } = string.Empty;