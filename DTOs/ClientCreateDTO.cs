public class ClientCreateDTO
{
    public string RaisonSociale { get; set; } = string.Empty;
    public string? Siret { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string? Pays { get; set; } = "FR";
    public string? SiteWeb { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Secteur { get; set; }

    // Option : créer un contact principal en même temps
    public ContactCreateDTO? ContactPrincipal { get; set; }
}