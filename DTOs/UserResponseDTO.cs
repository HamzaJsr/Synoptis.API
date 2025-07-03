using Synoptis.API.DTOs;

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public DateTime CreeLe { get; set; }

    public ICollection<AppelOffreShortDTO> AppelOffres { get; set; } = new List<AppelOffreShortDTO>();

    // Ajoute ça :
    public List<UserShortDTO>? Collaborateurs { get; set; } // Pour RA
    public UserShortDTO? Responsable { get; set; } // Pour CA/Secrétaire
    public List<UserShortDTO>? Collegues { get; set; } // Pour CA/Secrétaire
}
