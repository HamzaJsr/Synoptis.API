using Synoptis.API.DTOs;
using Synoptis.API.Enums;

namespace Synoptis.API.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.ResponsableAgence;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création

        public ICollection<AppelOffre>? AppelOffres { get; set; } = new List<AppelOffre>();

        // 🔥 Relations hiérarchiques
        public Guid? ResponsableId { get; set; }
        public User? Responsable { get; set; }
        public ICollection<User> Collaborateurs { get; set; } = new List<User>();
    }
}




