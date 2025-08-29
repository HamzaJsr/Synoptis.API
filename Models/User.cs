using System.ComponentModel.DataAnnotations;
using Synoptis.API.Enums;

namespace Synoptis.API.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;

        [EmailAddress, Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasse { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.ResponsableAgence;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // 🔗 Multi-tenant
        [Required]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public ICollection<AppelOffre>? AppelOffres { get; set; } = new List<AppelOffre>();

        // 🔥 Relations hiérarchiques
        public Guid? ResponsableId { get; set; } // Foreign Key
        public User? Responsable { get; set; }
        public ICollection<User> Collaborateurs { get; set; } = new List<User>();

        //Ajout pour lier avec les documents d'appel d'offre (je vois pas l'utulité pour l'instant mais on verra plus tard)
        public ICollection<DocumentAppelOffre> DocumentsDeposes { get; set; } = new List<DocumentAppelOffre>();
    }
}
