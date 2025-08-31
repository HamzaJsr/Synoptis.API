
using System.ComponentModel.DataAnnotations;
using Synoptis.API.Validation;

namespace Synoptis.API.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(256)]
        public string RaisonSociale { get; set; } = string.Empty;

        [Siret] // ← optionnel mais valide si présent
        public string Siret { get; set; } = string.Empty;

        [StringLength(256)]
        public string Adresse { get; set; } = string.Empty;

        [StringLength(128)]
        public string Ville { get; set; } = string.Empty;

        [RegularExpression(@"^\d{5}$", ErrorMessage = "Le code postal doit contenir 5 chiffres.")]
        public string CodePostal { get; set; } = string.Empty;

        [StringLength(128)]
        public string Pays { get; set; } = string.Empty;

        [StringLength(128)]
        public string FormeJuridique { get; set; } = string.Empty;

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // 🔗 Relations
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<AppelOffre> AppelsOffre { get; set; } = new List<AppelOffre>();

        public ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}
