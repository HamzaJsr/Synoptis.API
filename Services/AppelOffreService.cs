
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

            // On transforme le DTO reçu en entité AppelOffre car on ne pourra envoyer en BDD que des type AppelOffre
            var newAppelOffre = new AppelOffre
            {
                Titre = dto.Titre,
                Description = dto.Description,
                NomClient = dto.NomClient,
                DateLimite = dto.DateLimite,
                CreeLe = DateTime.UtcNow,
                CreatedById = userId,
                CompanyId = CompanyId
            };

            //J'ajoute dans la bdd
            _dbContext.AppelOffres.Add(newAppelOffre);

            // Je save le changement de maniere async
            await _dbContext.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = newAppelOffre.Id,
                Titre = newAppelOffre.Titre,
                Description = newAppelOffre.Description,
                NomClient = newAppelOffre.NomClient,
                DateLimite = newAppelOffre.DateLimite,
                CreeLe = newAppelOffre.CreeLe,
                Statut = _enumToStringService.StatutAoEnumService(newAppelOffre.Statut)
            };
        }


        // methode pour modifier un AO
        public async Task<AppelOffreResponseDTO?> UpdateAppelOffre(Guid id, AppelOffreUpdateDTO dto)
        {
            var appelOffreToUpdate = await _dbContext.AppelOffres.FirstOrDefaultAsync(ao => ao.Id == id && ao.CompanyId == CompanyId);

            if (appelOffreToUpdate == null)
            {
                return null; // ou NotFound() dans un contrôleur
            }

            if (dto.Titre != null)
            {
                appelOffreToUpdate.Titre = dto.Titre;
            }

            if (dto.Description != null)
            {
                appelOffreToUpdate.Description = dto.Description;
            }

            if (dto.DateLimite != null)
            {
                appelOffreToUpdate.DateLimite = (DateTime)dto.DateLimite;
            }

            if (dto.NomClient != null)
            {
                appelOffreToUpdate.NomClient = dto.NomClient;
            }

            // Je save le changement de maniere async
            await _dbContext.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = appelOffreToUpdate.Id,
                Titre = appelOffreToUpdate.Titre,
                Description = appelOffreToUpdate.Description,
                NomClient = appelOffreToUpdate.NomClient,
                DateLimite = appelOffreToUpdate.DateLimite,
                CreeLe = appelOffreToUpdate.CreeLe,
                Statut = _enumToStringService.StatutAoEnumService(appelOffreToUpdate.Statut)
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
                NomClient = appelOffreToDelete.NomClient,
                DateLimite = appelOffreToDelete.DateLimite,
                CreeLe = appelOffreToDelete.CreeLe,
                Statut = _enumToStringService.StatutAoEnumService(appelOffreToDelete.Statut)
            };
        }

    }
}