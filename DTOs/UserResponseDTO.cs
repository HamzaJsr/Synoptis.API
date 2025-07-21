using Synoptis.API.DTOs;

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public DateTime CreeLe { get; set; }

    public ICollection<AppelOffreShortDTO> AppelOffres { get; set; } = new List<AppelOffreShortDTO>();

    public List<UserShortDTO>? Collaborateurs { get; set; } = new(); // Pour RA
    public UserShortDTO? Responsable { get; set; } = new(); // Pour CA/Secrétaire
    public List<UserShortDTO>? Collegues { get; set; } = new(); // Pour CA/Secrétaire
}
