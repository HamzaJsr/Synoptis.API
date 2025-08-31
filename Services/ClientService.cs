// Services/ClientService.cs
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.Models;
using Synoptis.API.Services.Abstractions;

public interface IClientService
{
    Task<List<ClientRechercheDTO>> SearchAsync(string q, int take = 10);
    Task<ClientResponseDTO> CreateAsync(ClientCreateDTO dto);
    Task<ClientResponseDTO?> GetAsync(Guid id);
}

public class ClientService : TenantAwareService, IClientService
{
    private readonly SynoptisDbContext _db;

    public ClientService(SynoptisDbContext db, IHttpContextAccessor http) : base(http)
    {
        _db = db;
    }

    public async Task<List<ClientRechercheDTO>> SearchAsync(string q, int take = 10)
    {
        q = (q ?? string.Empty).Trim();
        if (q.Length == 0) return new();

        // PostgreSQL: ILIKE pour recherche case-insensitive
        return await _db.Clients
            .Where(c => c.CompanyId == CompanyId &&
                        (EF.Functions.ILike(c.RaisonSociale, $"%{q}%")
                         || (c.Siret != null && EF.Functions.ILike(c.Siret, $"{q}%"))))
            .OrderBy(c => c.RaisonSociale)
            .Take(take)
            .Select(c => new ClientRechercheDTO { Id = c.Id, RaisonSociale = c.RaisonSociale, Siret = c.Siret })
            .ToListAsync();
    }

    public async Task<ClientResponseDTO?> GetAsync(Guid id)
    {
        var c = await _db.Clients
            .Include(x => x.Contacts)
            .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == CompanyId);

        if (c == null) return null;

        return new ClientResponseDTO
        {
            Id = c.Id,
            RaisonSociale = c.RaisonSociale,
            Siret = c.Siret,
            Adresse = c.Adresse,
            Ville = c.Ville,
            CodePostal = c.CodePostal,
            Pays = c.Pays,
            SiteWeb = c.SiteWeb,
            Telephone = c.Telephone,
            Email = c.Email,
            Secteur = c.Secteur,
            Contacts = c.Contacts.Select(k => new ContactItemDTO
            {
                Id = k.Id,
                Prenom = k.Prenom,
                Nom = k.Nom,
                Email = k.Email,
                Telephone = k.Telephone,
                Fonction = k.Fonction,
                Decisionnaire = k.Decisionnaire
            }).ToList()
        };
    }

    public async Task<ClientResponseDTO> CreateAsync(ClientCreateDTO dto)
    {
        // (Option) Anti-doublon simple sur (CompanyId, RaisonSociale)
        var exists = await _db.Clients.AnyAsync(c =>
            c.CompanyId == CompanyId && c.RaisonSociale.ToLower() == dto.RaisonSociale.Trim().ToLower()
        );
        if (exists) throw new InvalidOperationException("Un client avec cette raison sociale existe déjà.");

        var client = new Client
        {
            CompanyId = CompanyId,
            RaisonSociale = dto.RaisonSociale.Trim(),
            Siret = string.IsNullOrWhiteSpace(dto.Siret) ? null : dto.Siret.Trim(),
            Siren = string.IsNullOrWhiteSpace(dto.Siret) ? null : dto.Siret.Trim().Length >= 9 ? dto.Siret.Trim().Substring(0, 9) : null,
            Adresse = dto.Adresse?.Trim(),
            Ville = dto.Ville?.Trim(),
            CodePostal = dto.CodePostal?.Trim(),
            Pays = string.IsNullOrWhiteSpace(dto.Pays) ? "FR" : dto.Pays.Trim(),
            SiteWeb = dto.SiteWeb?.Trim(),
            Telephone = dto.Telephone?.Trim(),
            Email = dto.Email?.Trim(),
            Secteur = dto.Secteur?.Trim()
        };

        if (dto.ContactPrincipal is not null)
        {
            client.Contacts.Add(new ContactClient
            {
                CompanyId = CompanyId,
                Prenom = dto.ContactPrincipal.Prenom.Trim(),
                Nom = dto.ContactPrincipal.Nom.Trim(),
                Email = dto.ContactPrincipal.Email.Trim(),
                Telephone = dto.ContactPrincipal.Telephone?.Trim(),
                Fonction = dto.ContactPrincipal.Fonction?.Trim(),
                Decisionnaire = dto.ContactPrincipal.Decisionnaire
            });
        }

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return await GetAsync(client.Id) ?? throw new Exception("Création client : lecture impossible.");
    }
}
