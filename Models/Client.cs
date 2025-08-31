// Models/Client.cs
using System.ComponentModel.DataAnnotations;
using Synoptis.API.Validation; // pour [Siret]

namespace Synoptis.API.Models
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Multi-tenant
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        [MaxLength(256)]
        public string RaisonSociale { get; set; } = string.Empty;

        [Siret] public string? Siret { get; set; }  // 14 chiffres + Luhn (optionnel)
        [MaxLength(9)] public string? Siren { get; set; }  // (les 9 premiers du SIRET si tu veux stocker)
        [MaxLength(32)] public string? TvaIntracom { get; set; } // ex: FRxx...

        [MaxLength(256)] public string? Adresse { get; set; }
        [MaxLength(128)] public string? Ville { get; set; }
        [RegularExpression(@"^\d{5}$")] public string? CodePostal { get; set; }
        [MaxLength(64)] public string? Pays { get; set; } = "FR";

        [Url] public string? SiteWeb { get; set; }
        [Phone] public string? Telephone { get; set; }
        [EmailAddress] public string? Email { get; set; }

        [MaxLength(128)] public string? Secteur { get; set; } // (optionnel: BTP, Retail…)

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
        public DateTime? ModifieLe { get; set; }

        public ICollection<ContactClient> Contacts { get; set; } = new List<ContactClient>();
        public ICollection<AppelOffre> AppelsOffre { get; set; } = new List<AppelOffre>();
    }
}
