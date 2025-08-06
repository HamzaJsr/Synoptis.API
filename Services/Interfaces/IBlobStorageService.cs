
using Synoptis.API.DTOs;

namespace Synoptis.API.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<AppelOffreDocumentDTO> UploadDocumentAsync(UploadDocumentRequest request, string userId);

        Task<AppelOffreDocumentDTO?> DeleteDocumentAsync(Guid documentId);
    }
}