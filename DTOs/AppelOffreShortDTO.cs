
using Synoptis.API.Models;

namespace Synoptis.API.DTOs
{
    public class AppelOffreShortDTO
    {
        public Guid Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateLimite { get; set; }
        public Client Client { get; set; } = null!;
        public string Statut { get; set; } = "En cours";
        public DateTime CreeLe { get; set; }

        // PAS de CreatedBy ni CreatedById
    }
}