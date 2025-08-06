
namespace Synoptis.API.DTOs
{
    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public Guid AppelOffreId { get; set; }
    }

}