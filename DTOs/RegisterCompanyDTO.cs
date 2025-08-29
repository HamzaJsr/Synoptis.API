// ✅ Infos société envoyées au register
namespace Synoptis.API.DTOs
{
    public class RegisterCompanyDTO
    {
        public string RaisonSociale { get; set; } = string.Empty;
        public string? Siret { get; set; }          // optionnel (validé par [Siret] côté entité)
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }     // optionnel (Regex côté entité)
        public string? Pays { get; set; }
        public string? FormeJuridique { get; set; }
    }
}
