
using System.ComponentModel.DataAnnotations.Schema;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;

namespace Synoptis.API.Models
{
    public class AppelOffre
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement

        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres

        public Guid? ClientId { get; set; }   // ✅ FK
        public Client? Client { get; set; }   // navigation

        public string Description { get; set; } = string.Empty; // Description globale

        public DateTime DateLimite { get; set; }


        public StatutAppelOffre Statut { get; set; } = StatutAppelOffre.EnCours;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        // 🔗 Multi-tenant
        public Guid? CompanyId { get; set; }          // ← FK
        public Company Company { get; set; } = null!; // ← navigation

        //Ajout pour lier avec les documents d'appel d'offre
        public ICollection<DocumentAppelOffre> Documents { get; set; } = new List<DocumentAppelOffre>();

    }
}
