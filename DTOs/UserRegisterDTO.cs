
namespace Synoptis.API.DTOs
{
    public class UserRegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
    }
}