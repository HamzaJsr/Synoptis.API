
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Synoptis.API.Models;

namespace Synoptis.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config) => _config = config;
        /// <summary>
        /// Methode qui permet de generer un token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateJwt(User user)
        {
            //recuperer la cles secrete depuis appconfig grace a _config
            //👉 Ça évite le warning et te protège d’une erreur silencieuse si la clé est mal configurée.
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret key is missing.");

            var key = Encoding.UTF8.GetBytes(jwtKey);

            //faire la description du token 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // On commence par les claims, les donnée contenu dans le token Subject
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("companyId", user.CompanyId.ToString())
                }
                ),
                //Puis on met le delais d'expiration
                Expires = DateTime.UtcNow.AddHours(4),

                // Ensuite on delcare comment doit etre signé le token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                // Ici on met "Issuer" (émetteur) = qui a créé/généré le token
                Issuer = _config["Jwt:Issuer"],
                // Ici on met "Audience" (audience) = pour qui est prévu le token
                Audience = _config["Jwt:Audience"]
            };

            //Et la pour finir on cree une instance de JwtSecurityTokenHandler le gestionnaire JWT pour générer, lire et valider les tokens
            var tokenHandler = new JwtSecurityTokenHandler();
            // Et la je cree le token 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // et la je return le token au format string avec WriteToken de tokenHandler
            return tokenHandler.WriteToken(token);
        }
    }
}