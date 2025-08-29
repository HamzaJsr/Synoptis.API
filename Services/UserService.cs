

using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;
using Synoptis.API.Services.Interfaces;

namespace Synoptis.API.Services
{
    public class UserService : IUserService
    {

        private readonly SynoptisDbContext _context;
        private readonly TokenService _tokenService;

        private readonly PasswordHasher<User> _hasher = new();

        public UserService(SynoptisDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;

        }

        /// <summary>
        /// Cette fonction permet de créer le compte utulisateur du responsable 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterDTO dto)
        {
            // 1) Email unique
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return new UserRegisterResponseDTO { Success = false, Message = "Email déjà utilisé" };

            // 2) Construire Company
            var company = new Company
            {
                RaisonSociale = dto.Company.RaisonSociale?.Trim() ?? string.Empty,
                Siret = dto.Company.Siret?.Trim() ?? string.Empty,
                Adresse = dto.Company.Adresse?.Trim() ?? string.Empty,
                Ville = dto.Company.Ville?.Trim() ?? string.Empty,
                CodePostal = dto.Company.CodePostal?.Trim() ?? string.Empty,
                Pays = dto.Company.Pays?.Trim() ?? string.Empty,
                FormeJuridique = dto.Company.FormeJuridique?.Trim() ?? string.Empty
            };

            // 3) Construire User
            var user = new User
            {
                Prenom = dto.Prenom.Trim(),
                Nom = dto.Nom.Trim(),
                Email = dto.Email.Trim(),
                Role = UserRole.ResponsableAgence
                // CompanyId sera posé après la création de Company
            };
            user.MotDePasse = _hasher.HashPassword(user, dto.MotDePasse);

            // 4) Transaction explicite : TOUT ou RIEN
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();      // Étape 1 OK → Company insérée

                user.CompanyId = company.Id;            // Lier l'utilisateur au tenant
                _context.Users.Add(user);
                await _context.SaveChangesAsync();      // Étape 2 OK → User inséré

                await tx.CommitAsync();                 // ✅ On valide tout
            }
            catch
            {
                await tx.RollbackAsync();               // ❌ On annule tout si une étape échoue
                throw;                                  // On laisse remonter (middleware/controller gère)
            }

            return new UserRegisterResponseDTO
            {
                Success = true,
                Message = "Utilisateur et société créés.",
            };
        }


        public async Task<AuthResultDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
                return new AuthResultDTO { ErrorMessage = "Utilisateur introuvable" };

            var result = _hasher.VerifyHashedPassword(user, user.MotDePasse, dto.MotDePasse);

            if (result == PasswordVerificationResult.Failed)
                return new AuthResultDTO { ErrorMessage = "Mot de passe incorrect" };

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.MotDePasse = _hasher.HashPassword(user, dto.MotDePasse);
                await _context.SaveChangesAsync();
            }


            var token = _tokenService.GenerateJwt(user);

            return new AuthResultDTO { Token = token };
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
            .Include(u => u.AppelOffres)
            .Include(u => u.Responsable)
            .Include(u => u.Collaborateurs)
            .ToListAsync();

            return users.Adapt<List<UserResponseDTO>>();

        }

        /// <summary>
        /// Récupère l’utilisateur identifié par <paramref name="userId"/> en incluant :
        ///  - ses appels d’offres (Include(u => u.AppelOffres)),  
        ///  - ses collaborateurs directs (Include(u => u.Collaborateurs)) pour un ResponsableAgence,  
        ///  - son responsable et les collaborateurs de ce responsable (Include(u => u.Responsable).ThenInclude(r => r!.Collaborateurs))  
        ///    afin que le DTO final contienne à la fois Collaborateurs, Responsable et Collegues.
        /// </summary>
        public async Task<UserResponseDTO?> GetUserAsync(Guid userId)
        {
            var user = await _context.Users
            .Include(u => u.AppelOffres)
            .Include(u => u.Responsable)
                .ThenInclude(r => r!.Collaborateurs)  // ← essentiel pour charger les collègues
            .Include(u => u.Collaborateurs)
            .FirstOrDefaultAsync(u => u.Id == userId);


            if (user is null)
            {
                return null;
            }

            return user?.Adapt<UserResponseDTO>();

        }

        public async Task<UserResponseDTO> CreateUserByResponsableAsync(Guid responsableId, CreateUserDTO dto)
        {
            var responsable = await _context.Users.FindAsync(responsableId);

            if (responsable == null || responsable.Role != UserRole.ResponsableAgence)
                throw new UnauthorizedAccessException("Seul un RA peut créer un utilisateur.");

            var nouvelUtilisateur = new User
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Role = dto.Role,
                ResponsableId = responsableId
            };

            nouvelUtilisateur.MotDePasse = _hasher.HashPassword(nouvelUtilisateur, dto.MotDePasse);

            _context.Users.Add(nouvelUtilisateur);
            await _context.SaveChangesAsync();

            return nouvelUtilisateur.Adapt<UserResponseDTO>();
        }

        public async Task<UserResponseDTO?> GetMeAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.AppelOffres)
                .Include(u => u.Collaborateurs)
                .Include(u => u.Responsable)
                    .ThenInclude(r => r!.Collaborateurs)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return user.Adapt<UserResponseDTO>();
        }

    }
}
