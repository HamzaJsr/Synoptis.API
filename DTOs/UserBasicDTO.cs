
using Synoptis.API.Enums;

namespace Synoptis.API.DTOs
{
    public class UserBasicDTO
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreeLe { get; set; }
        // PAS de AppelOffres ici !
    }
}