using Mapster;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;
using Synoptis.API.Services;

namespace Synoptis.API.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 1️⃣ Mapping User → UserShortDTO (pour listes simples, ex: collègues, collaborateurs, responsable)
            config.NewConfig<User, UserShortDTO>()
                // On convertit l'enum Role en string pour le front
                .Map(dest => dest.Role, src => EnumToStringService.RoleUserEnumServiceStatic(src.Role));

            // « Quand je convertis un AppelOffre en AppelOffreShortDTO, je veux mapper Statut manuellement. »
            config.NewConfig<AppelOffre, AppelOffreShortDTO>()
                // Mapster va automatiquement copier les propriétés qui ont le même nom et le même type…
                // Mais tu peux surcharger certains champs avec .Map(...).
                // ➡️ Tu dis :
                // « Pour la propriété Statut du DTO (dest) destination, voici comment la remplir à partir du src source. »
                // ➡️ Tu dis :
                // « Prends la valeur Statut de l’entité (qui est sûrement un enum)
                // et passe-la à une fonction de conversion pour obtenir une chaîne lisible. »
                .Map(dest => dest.Statut, src => EnumToStringService.StatutAoEnumServiceStatic(src.Statut));

            //Configuration pour mapster pour utiliser la methode pour rendre les enum en string 
            config.NewConfig<User, UserResponseDTO>()
                .Map(dest => dest.Role, src => EnumToStringService.RoleUserEnumServiceStatic(src.Role))

                // b) Mappe la liste des AO de l'utilisateur en version simplifiée (AppelOffreShortDTO)
                .Map(dest => dest.AppelOffres,
                    src => src.AppelOffres != null
                        ? src.AppelOffres.Adapt<List<AppelOffreShortDTO>>() // Adapt = Mapster fait la conversion
                        : new List<AppelOffreShortDTO>())

                // c) Si c'est un ResponsableAgence → mappe ses collaborateurs en UserShortDTO
                .Map(dest => dest.Collaborateurs,
                    src => src.Role == UserRole.ResponsableAgence
                        ? src.Collaborateurs.Adapt<List<UserShortDTO>>() // Prend tous ses collab et mappe vers UserShortDTO
                        : null)

                // d) Si ce n'est PAS un ResponsableAgence ET il a un responsable → mappe son responsable
                .Map(dest => dest.Responsable,
                    src => src.Role != UserRole.ResponsableAgence && src.Responsable != null
                        ? src.Responsable.Adapt<UserShortDTO>() // Mappe le responsable du CA/secrétaire vers UserShortDTO
                        : null)

                // e) Si ce n'est PAS un ResponsableAgence ET il a un responsable → mappe ses collègues
                .Map(dest => dest.Collegues,
                    src => src.Role != UserRole.ResponsableAgence && src.Responsable != null
                        ? src.Responsable.Collaborateurs // Prend tous les collaborateurs du même responsable...
                            .Where(c => c.Id != src.Id)   // ... sauf lui-même
                            .Adapt<List<UserShortDTO>>()  // Mappe vers UserShortDTO
                        : null
                );
        }

    }
}
