
using System.ComponentModel.DataAnnotations.Schema;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;

namespace Synoptis.API.Models
{
    public class AppelOffre
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement

        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres
        public string NomClient { get; set; } = string.Empty; // Le client qui a publié l’AO
        public string Description { get; set; } = string.Empty; // Description globale
        [Column(TypeName = "timestamp without time zone")]
        public DateTime DateLimite { get; set; }


        public StatutAppelOffre Statut { get; set; } = StatutAppelOffre.EnCours;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

    }
}

