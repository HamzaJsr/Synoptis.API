
using Synoptis.API.Enums;

namespace Synoptis.API.Models
{
    public class AppelOffre
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement

        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres
        public string NomClient { get; set; } = string.Empty; // Le client qui a publié l’AO
        public string Description { get; set; } = string.Empty; // Description globale
        public DateTime DateLimite { get; set; } // Date limite de réponse

        public StatutAppelOffre Statut { get; set; } = StatutAppelOffre.EnCours;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

    }
}

//Implication modèle de données :
//Ajouter CreatedByUserId (clé étrangère vers User)

//Ajouter table User avec :

//Id, Nom, Role (Chargé d’affaires, Responsable, Secrétaire)

//AgenceId (pour regroupement par agence)

//Ajouter table Agence si elle n’existe pas