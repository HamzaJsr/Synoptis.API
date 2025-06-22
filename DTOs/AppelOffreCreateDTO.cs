using Synoptis.API.Validation;

namespace Synoptis.API.DTOs
{
    //les champs utiles à l’envoi depuis un formulaire 
    public class AppelOffreCreateDTO
    {
        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres
        public string Description { get; set; } = string.Empty; // Description globale
        [FutureDate(ErrorMessage = "La date limite doit être dans le futur.")]
        public DateTime DateLimite { get; set; } // Date limite de réponse
        public string NomClient { get; set; } = string.Empty; // Le client qui a publié l’AO

    }
}