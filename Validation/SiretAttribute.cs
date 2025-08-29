// ✅ On utilise DataAnnotations pour créer un attribut de validation personnalisé
using System.ComponentModel.DataAnnotations;

namespace Synoptis.API.Validation
{
    /// <summary>
    /// Valide un SIRET : 14 chiffres + contrôle Luhn.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)] // 👉 Cet attribut pourra être appliqué sur une propriété (comme Company.Siret)
    public sealed class SiretAttribute : ValidationAttribute // 👉 On hérite de ValidationAttribute pour créer notre règle custom
    {
        // Méthode appelée automatiquement par le framework quand il valide la propriété
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // On récupère la valeur (cast en string) et on enlève les espaces éventuels
            var s = (value as string)?.Trim();

            // Si la valeur est null ou vide → pas d’erreur (ça permet de rendre le champ optionnel)
            if (string.IsNullOrWhiteSpace(s))
                return ValidationResult.Success;

            // Vérification 1 : longueur = 14 ET uniquement des chiffres
            if (s.Length != 14 || !s.All(char.IsDigit))
                return new ValidationResult("Le SIRET doit contenir exactement 14 chiffres.");

            // Vérification 2 : Contrôle de validité avec l’algorithme de Luhn
            int sum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                // On convertit le caractère en chiffre
                int digit = s[i] - '0';

                // Convention du SIRET : on double les chiffres en position paire (index 0-based)
                if ((i % 2) == 0)
                    digit *= 2;

                // Si le double dépasse 9, on soustrait 9 (règle de Luhn)
                if (digit > 9)
                    digit -= 9;

                // On ajoute le résultat à la somme totale
                sum += digit;
            }

            // La somme totale doit être un multiple de 10
            if (sum % 10 != 0)
                return new ValidationResult("SIRET invalide (contrôle Luhn).");

            // Si toutes les règles sont passées → validation OK
            return ValidationResult.Success;
        }
    }
}
