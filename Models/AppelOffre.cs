using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synoptis.API.Models
{
    public class AppelOffre
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement

        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres
        public string Description { get; set; } = string.Empty; // Description globale
        public DateTime DateLimite { get; set; } // Date limite de réponse
        public string NomClient { get; set; } = string.Empty; // Le client qui a publié l’AO

        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création
    }
}