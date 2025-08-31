public class ContactItemDTO
{
    public Guid Id { get; set; }
    public string Prenom { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Fonction { get; set; }
    public bool Decisionnaire { get; set; }
}