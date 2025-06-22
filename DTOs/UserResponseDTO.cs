using Synoptis.API.Enums;


namespace Synoptis.API.DTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création

        public ICollection<AppelOffreShortDTO> AppelOffres { get; set; } = new List<AppelOffreShortDTO>();
    }
}