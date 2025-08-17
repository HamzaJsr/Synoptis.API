namespace Synoptis.API.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RaisonSociale { get; set; } = string.Empty;
        public string Siret { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
        public string Pays { get; set; } = string.Empty;
        public string FormeJuridique { get; set; } = string.Empty;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // 🔗 Relations
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<AppelOffre> AppelsOffre { get; set; } = new List<AppelOffre>();

    }
}
