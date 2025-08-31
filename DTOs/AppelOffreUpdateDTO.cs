using Synoptis.API.Models;
using Synoptis.API.Validation;

namespace Synoptis.API.DTOs
{
    public class AppelOffreUpdateDTO
    {
        public string? Titre { get; set; } // Titre de l’appel d’offres
        public string? Description { get; set; } // Description globale
        [FutureDate(ErrorMessage = "La date limite doit être dans le futur.")]
        public DateTime? DateLimite { get; set; } // Date limite de réponse
        public Guid? ClientId { get; set; } // 🔑 on change le client lié // Le client qui a publié l’AO
    }
}