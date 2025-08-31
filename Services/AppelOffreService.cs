
using Azure.Storage.Blobs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;
using Synoptis.API.Services.Abstractions;
using Synoptis.API.Services.Interfaces;


namespace Synoptis.API.Services
{
    public class AppelOffreService : TenantAwareService, IAppelOffreService
    {

        private readonly SynoptisDbContext _dbContext;
        private readonly EnumToStringService _enumToStringService;

        private readonly BlobServiceClient _blobServiceClient;//Client
        private readonly BlobContainerClient _blobContainerClient; //Container

        private readonly BlobStorageService _blobService;

        public AppelOffreService(SynoptisDbContext dbContext, EnumToStringService enumToStringService, BlobServiceClient blobServiceClient, IConfiguration config, BlobStorageService blobService, IHttpContextAccessor http) : base(http)
        {
            _dbContext = dbContext;
            _enumToStringService = enumToStringService;
            _blobServiceClient = blobServiceClient;//Client
            var containerName = config["AzureBlobStorage:ContainerName"];
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            _blobService = blobService;

        }

        // methode pour recuperer tout les AO
        public async Task<IEnumerable<AppelOffreResponseDTO>> GetAllAppelOffresAsync()
        {

            var allAppelOffres = await _dbContext.AppelOffres
            .Where(ao => ao.CompanyId == CompanyId)
            .Include(ao => ao.CreatedBy)
            .Include(a => a.Client)
            .Include(ao => ao.Documents)
            .ToListAsync();

            // var resultDto = allAppelOffres.Select(ao =>
            // {
            //     var statutFinal = ao.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : ao.Statut;

            //     return new AppelOffreResponseDTO
            //     {
            //         Id = ao.Id,
            //         Titre = ao.Titre,
            //         Description = ao.Description,
            //         NomClient = ao.NomClient,
            //         DateLimite = ao.DateLimite,
            //         CreeLe = ao.CreeLe,
            //         Statut = _enumToStringService.StatutAoEnumService(statutFinal),
            //         CreatedById = ao.CreatedById,
            //         CreatedBy = new UserBasicDTO
            //         {
            //             Id = ao.CreatedBy.Id,
            //             Nom = ao.CreatedBy.Nom,
            //             Email = ao.CreatedBy.Email,
            //             Role = ao.CreatedBy.Role,
            //             CreeLe = ao.CreatedBy.CreeLe
            //         }
            //     };
            // }).ToList();

            // return resultDto;

            return allAppelOffres.Adapt<IEnumerable<AppelOffreResponseDTO>>();
        }


        // methode pour recuperer un AO grace a son ID depuis la bdd
        public async Task<AppelOffreResponseDTO?> GetAppelOffreByIdAsync(Guid id)
        {
            var appelOffre = await _dbContext.AppelOffres
            .Include(ao => ao.CreatedBy)
            .Include(ao => ao.Documents)
            .FirstOrDefaultAsync(ao => ao.Id == id && ao.CompanyId == CompanyId);

            if (appelOffre is null)
            {
                return null;
            }

            var dto = appelOffre.Adapt<AppelOffreResponseDTO>();

            // Statut calculé côté service
            var statutFinal = appelOffre.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : appelOffre.Statut;
            dto.Statut = _enumToStringService.StatutAoEnumService(statutFinal);

            // SAS par document
            foreach (var d in dto.Documents)
            {
                var blobName = $"{appelOffre.Id}/{d.TypeDocument}/{d.NomFichier}";
                d.Url = _blobService.GenerateSasUrl(blobName, TimeSpan.FromMinutes(10));
            }

            // var statutFinal = appelOffre.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : appelOffre.Statut;

            // return new AppelOffreResponseDTO
            // {
            //     Id = appelOffre.Id,
            //     Titre = appelOffre.Titre,
            //     Description = appelOffre.Description,
            //     NomClient = appelOffre.NomClient,
            //     DateLimite = appelOffre.DateLimite,
            //     CreeLe = appelOffre.CreeLe,
            //     Statut = _enumToStringService.StatutAoEnumService(statutFinal),
            //     CreatedById = appelOffre.CreatedById,
            //     CreatedBy = new UserBasicDTO
            //     {
            //         Id = appelOffre.CreatedBy.Id,
            //         Nom = appelOffre.CreatedBy.Nom,
            //         Email = appelOffre.CreatedBy.Email,
            //         Role = appelOffre.CreatedBy.Role,
            //         CreeLe = appelOffre.CreatedBy.CreeLe
            //     }
            // };

            return dto;

        }
        // methode pour ajouter un AO elle return un DTO pour le front
        public async Task<AppelOffreResponseDTO> CreateAppelOffreAsync(Guid userId, AppelOffreCreateDTO dto)
        {
            if (dto.ClientId.HasValue)
            {
                var ok = await _dbContext.Clients
                    .AnyAsync(c => c.Id == dto.ClientId.Value && c.CompanyId == CompanyId);
                if (!ok) throw new UnauthorizedAccessException("Client introuvable pour votre société.");
            }

            var entity = new AppelOffre
            {
                Titre = dto.Titre,
                Description = dto.Description,
                DateLimite = dto.DateLimite,
                CreeLe = DateTime.UtcNow,
                CreatedById = userId,
                CompanyId = CompanyId,
                ClientId = dto.ClientId             // ✅ pas d’objet, juste la FK
            };

            _dbContext.AppelOffres.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Charger pour la réponse
            await _dbContext.Entry(entity).Reference(e => e.CreatedBy).LoadAsync();
            await _dbContext.Entry(entity).Reference(e => e.Client).LoadAsync();
            await _dbContext.Entry(entity).Collection(e => e.Documents).LoadAsync();

            var statutFinal = entity.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : entity.Statut;

            return new AppelOffreResponseDTO
            {
                Id = entity.Id,
                Titre = entity.Titre,
                Description = entity.Description,
                DateLimite = entity.DateLimite,
                CreeLe = entity.CreeLe,
                CreatedById = entity.CreatedById,
                CreatedBy = entity.CreatedBy.Adapt<UserBasicDTO>(),
                Statut = _enumToStringService.StatutAoEnumService(statutFinal),

                ClientId = entity.ClientId,                                   // ✅
                ClientRaisonSociale = entity.Client?.RaisonSociale,           // ✅

                Documents = entity.Documents.Adapt<ICollection<AppelOffreDocumentDTO>>()
            };
        }



        // methode pour modifier un AO
        public async Task<AppelOffreResponseDTO?> UpdateAppelOffre(Guid id, AppelOffreUpdateDTO dto)
        {
            var entity = await _dbContext.AppelOffres
                .Include(a => a.CreatedBy)
                .Include(a => a.Client)
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CompanyId);

            if (entity is null) return null;

            if (dto.Titre != null) entity.Titre = dto.Titre;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.DateLimite != null) entity.DateLimite = dto.DateLimite.Value;

            if (dto.ClientId.HasValue)                       // ✅ remplace ton ancien code
            {
                var ok = await _dbContext.Clients
                    .AnyAsync(c => c.Id == dto.ClientId.Value && c.CompanyId == CompanyId);
                if (!ok) throw new UnauthorizedAccessException("Client introuvable pour votre société.");
                entity.ClientId = dto.ClientId.Value;        // ✅ on modifie la FK
                await _dbContext.Entry(entity).Reference(e => e.Client).LoadAsync(); // recharge pour la réponse
            }

            await _dbContext.SaveChangesAsync();

            var statutFinal = entity.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : entity.Statut;

            return new AppelOffreResponseDTO
            {
                Id = entity.Id,
                Titre = entity.Titre,
                Description = entity.Description,
                DateLimite = entity.DateLimite,
                CreeLe = entity.CreeLe,
                CreatedById = entity.CreatedById,
                CreatedBy = entity.CreatedBy.Adapt<UserBasicDTO>(),
                Statut = _enumToStringService.StatutAoEnumService(statutFinal),

                ClientId = entity.ClientId,                                   // ✅
                ClientRaisonSociale = entity.Client?.RaisonSociale,           // ✅

                Documents = entity.Documents.Adapt<ICollection<AppelOffreDocumentDTO>>()
            };
        }


        // methode pour supprimer un AO 
        public async Task<AppelOffreResponseDTO?> DeleteAppelOffreAsync(Guid id)
        {
            var appelOffreToDelete = await _dbContext.AppelOffres
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(ao => ao.Id == id && ao.CompanyId == CompanyId);

            if (appelOffreToDelete is null)
            {
                return null;
            }

            //Supprimer tout les documents de l'ao dabord et dabord d'azure
            foreach (var doc in appelOffreToDelete.Documents)
            {
                var blobName = $"{doc.AppelOffreId}/{doc.TypeDocument}/{doc.NomFichier}";
                var blobClient = _blobContainerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }

            // La je supprime les documents lie a l'ao cote BDD
            _dbContext.DocumentsAppelOffre.RemoveRange(appelOffreToDelete.Documents);

            // je supprime l'ao de la bdd 
            _dbContext.AppelOffres.Remove(appelOffreToDelete);

            // Je save le changement de maniere async
            await _dbContext.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = appelOffreToDelete.Id,
                Titre = appelOffreToDelete.Titre,
                Description = appelOffreToDelete.Description,
                ClientId = appelOffreToDelete.ClientId,
                DateLimite = appelOffreToDelete.DateLimite,
                CreeLe = appelOffreToDelete.CreeLe,
                Statut = _enumToStringService.StatutAoEnumService(appelOffreToDelete.Statut)
            };
        }

    }
}