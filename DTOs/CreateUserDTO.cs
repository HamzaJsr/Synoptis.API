using Synoptis.API.Enums;

// Crée un CreateUserDTO pour que le RA puisse ajouter des utilisateurs

namespace Synoptis.API.DTOs
{
    public class CreateUserDTO
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public UserRole Role { get; set; } // ChargeAffaire ou Secretaire
    }
}