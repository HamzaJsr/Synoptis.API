// DTOs/ClientUpdateDTO.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Synoptis.API.Validation;

public class ClientUpdateDTO
{
    // soit tu fournis l'ID du client existant…
    public Guid? Id { get; set; }

    // …ou tu patches des champs (tous optionnels)
    [MaxLength(256)]
    public string? RaisonSociale { get; set; }

    [Siret] public string? Siret { get; set; }
    [MaxLength(9)] public string? Siren { get; set; }
    [MaxLength(32)] public string? TvaIntracom { get; set; }

    [MaxLength(256)] public string? Adresse { get; set; }
    [MaxLength(128)] public string? Ville { get; set; }
    [RegularExpression(@"^\d{5}$")] public string? CodePostal { get; set; }
    [MaxLength(64)] public string? Pays { get; set; }

    [Url] public string? SiteWeb { get; set; }
    [Phone] public string? Telephone { get; set; }
    [EmailAddress] public string? Email { get; set; }

    [MaxLength(128)] public string? Secteur { get; set; }
}
