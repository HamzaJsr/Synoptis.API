
namespace Synoptis.API.DTOs
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}