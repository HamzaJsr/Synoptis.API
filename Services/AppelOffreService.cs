
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;
using Synoptis.API.Services.Interfaces;

namespace Synoptis.API.Services
{
    public class AppelOffreService : IAppelOffreService
    {
        private readonly SynoptisDbContext _context;

        public AppelOffreService(SynoptisDbContext context) => _context = context;

        public string GetStringStatutFromEnum(StatutAppelOffre statut)
        {
            return statut switch
            {
                StatutAppelOffre.EnCours => "En cours",
                StatutAppelOffre.SyntheseFaite => "Synthese faite",
                StatutAppelOffre.Go => "Go",
                StatutAppelOffre.NoGo => "No go",
                StatutAppelOffre.Expire => "Expiré",
                _ => "Inconnu"

            };
        }



        // methode pour recuperer tout les AO
        public async Task<IEnumerable<AppelOffreResponseDTO>> GetAllAppelOffresAsync()
        {

            var allAppelOffres = await _context.AppelOffres.ToListAsync();

            var result = new List<AppelOffreResponseDTO>();

            foreach (var ao in allAppelOffres)
            {
                var statutFinal = ao.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : ao.Statut;

                result.Add(new AppelOffreResponseDTO
                {
                    Id = ao.Id,
                    Titre = ao.Titre,
                    Description = ao.Description,
                    NomClient = ao.NomClient,
                    DateLimite = ao.DateLimite,
                    CreeLe = ao.CreeLe,
                    Statut = GetStringStatutFromEnum(statutFinal)
                });

            }

            return result;

        }

        // methode pour recuperer un AO grace a son ID depuis la bdd
        public async Task<AppelOffreResponseDTO?> GetAppelOffreByIdAsync(Guid id)
        {
            var appelOffre = await _context.AppelOffres.FindAsync(id);
            if (appelOffre is null)
            {
                return null;
            }

            var statutFinal = appelOffre.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : appelOffre.Statut;

            return new AppelOffreResponseDTO
            {
                Id = appelOffre.Id,
                Titre = appelOffre.Titre,
                Description = appelOffre.Description,
                NomClient = appelOffre.NomClient,
                DateLimite = appelOffre.DateLimite,
                CreeLe = appelOffre.CreeLe,
                Statut = GetStringStatutFromEnum(statutFinal)
            };
        }
        // methode pour ajouter un AO elle return un DTO pour le front
        public async Task<AppelOffreResponseDTO> CreateAppelOffreAsync(AppelOffreCreateDTO dto)
        {

            // On transforme le DTO reçu en entité AppelOffre car on ne pourra envoyer en BDD que des type AppelOffre
            var newAppelOffre = new AppelOffre
            {
                Titre = dto.Titre,
                Description = dto.Description,
                NomClient = dto.NomClient,
                DateLimite = dto.DateLimite,
                CreeLe = DateTime.UtcNow
            };

            //J'ajoute dans la bdd
            _context.AppelOffres.Add(newAppelOffre);

            // Je save le changement de maniere async
            await _context.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = newAppelOffre.Id,
                Titre = newAppelOffre.Titre,
                Description = newAppelOffre.Description,
                NomClient = newAppelOffre.NomClient,
                DateLimite = newAppelOffre.DateLimite,
                CreeLe = newAppelOffre.CreeLe,
                Statut = GetStringStatutFromEnum(newAppelOffre.Statut)
            };
        }


        // methode pour modifier un AO
        public async Task<AppelOffreResponseDTO?> UpdateAppelOffre(Guid id, AppelOffreUpdateDTO dto)
        {
            var appelOffreToUpdate = await _context.AppelOffres.FindAsync(id);

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
            await _context.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = appelOffreToUpdate.Id,
                Titre = appelOffreToUpdate.Titre,
                Description = appelOffreToUpdate.Description,
                NomClient = appelOffreToUpdate.NomClient,
                DateLimite = appelOffreToUpdate.DateLimite,
                CreeLe = appelOffreToUpdate.CreeLe,
                Statut = GetStringStatutFromEnum(appelOffreToUpdate.Statut)
            };
        }

        // methode pour supprimer un AO 
        public async Task<AppelOffreResponseDTO?> DeleteAppelOffreAsync(Guid id)
        {
            var appelOffreToDelete = await _context.AppelOffres.FindAsync(id);

            if (appelOffreToDelete is null)
            {
                return null;
            }

            // je supprime de la bdd 
            _context.AppelOffres.Remove(appelOffreToDelete);

            // Je save le changement de maniere async
            await _context.SaveChangesAsync();

            // La je retourne un DTO en reponse (au front client)

            return new AppelOffreResponseDTO
            {
                Id = appelOffreToDelete.Id,
                Titre = appelOffreToDelete.Titre,
                Description = appelOffreToDelete.Description,
                NomClient = appelOffreToDelete.NomClient,
                DateLimite = appelOffreToDelete.DateLimite,
                CreeLe = appelOffreToDelete.CreeLe,
                Statut = GetStringStatutFromEnum(appelOffreToDelete.Statut)
            };
        }

    }
}