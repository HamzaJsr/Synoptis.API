using Synoptis.API.DTOs;
using Synoptis.API.Models;

namespace Synoptis.API.Services.Interfaces
{
    public interface IAppelOffreService
    {
        // Récupérer tous les AO depuis la base
        Task<IEnumerable<AppelOffreResponseDTO>> GetAllAppelOffresAsync();

        // Récupérer un AO grace a son id depuis la base
        Task<AppelOffreResponseDTO?> GetAppelOffreByIdAsync(Guid id);

        // Créer un nouvel AO à partir des données reçues du front en passant par les DTO
        Task<AppelOffreResponseDTO> CreateAppelOffreAsync(AppelOffreCreateDTO dto);
    }
}