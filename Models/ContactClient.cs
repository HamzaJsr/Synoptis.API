// Models/ContactClient.cs
using System.ComponentModel.DataAnnotations;

namespace Synoptis.API.Models
{
    public class ContactClient
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Multi-tenant (pratique pour filtrer directement)
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        // Lien client
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        [MaxLength(128)] public string Prenom { get; set; } = string.Empty;
        [MaxLength(128)] public string Nom { get; set; } = string.Empty;
        [EmailAddress] public string Email { get; set; } = string.Empty;
        [Phone] public string? Telephone { get; set; }
        [MaxLength(128)] public string? Fonction { get; set; }  // ex: Acheteur, MOA…
        public bool Decisionnaire { get; set; } = false;

        public string? Notes { get; set; }
    }
}
