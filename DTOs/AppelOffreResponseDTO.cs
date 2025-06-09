namespace Synoptis.API.DTOs

{
    public class AppelOffreResponseDTO
    //  Sert à envoyer uniquement ces données dans un GET /api/ao 
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identifiant unique généré automatiquement

        public string Titre { get; set; } = string.Empty; // Titre de l’appel d’offres
        public string Description { get; set; } = string.Empty; // Description globale
        public DateTime DateLimite { get; set; } // Date limite de réponse
        public string NomClient { get; set; } = string.Empty; // Le client qui a publié l’AO

        public string Statut { get; set; } = "En cours";

        public DateTime CreeLe { get; set; } = DateTime.UtcNow; // Horodatage de création
    }
}