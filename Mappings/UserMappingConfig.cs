using Mapster;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;

namespace Synoptis.API.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 1️⃣ Mapping User → UserShortDTO (pour listes simples, ex: collègues, collaborateurs, responsable)
            config.NewConfig<User, UserShortDTO>()
                // On convertit l'enum Role en string pour le front
                .Map(dest => dest.Role, src => src.Role.ToString());

            // 2️⃣ Mapping principal : User → UserResponseDTO (le "gros" DTO pour /me)
            config.NewConfig<User, UserResponseDTO>()
                // a) Convertit l'enum Role en string pour plus de lisibilité côté front
                .Map(dest => dest.Role, src => src.Role.ToString())

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
