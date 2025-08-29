using System.ComponentModel.DataAnnotations;
using Synoptis.API.DTOs;

public class UserRegisterDTO
{
    // ⚠️ Astuce: n'initialise PAS à "", laisse null pour que [Required] détecte l'absence/champs vides.
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, StringLength(100)]
    public string Prenom { get; set; } = null!;   // 👤 ajouté

    [Required, StringLength(100)]
    public string Nom { get; set; } = null!;

    [Required, MinLength(8)]
    public string MotDePasse { get; set; } = null!;

    // On veut absolument ce bloc → [Required]
    [Required/*, ValidateComplexType*/]
    public RegisterCompanyDTO Company { get; set; } = null!;
}