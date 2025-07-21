using Mapster;
using Synoptis.API.DTOs;
using Synoptis.API.Enums;
using Synoptis.API.Models;
using Synoptis.API.Services;

namespace Synoptis.API.Mappings
{
    public class AppelOffreMappingConfig : IRegister
    {

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AppelOffreCreateDTO, AppelOffre>()
                .Map(dest => dest.DateLimite,
                     src => DateTime.SpecifyKind(src.DateLimite, DateTimeKind.Utc));

            config.NewConfig<AppelOffre, AppelOffreResponseDTO>()
        .Map(dest => dest.Statut,
             src => EnumToStringService.StatutAoEnumServiceStatic(
                 src.DateLimite < DateTime.UtcNow ? StatutAppelOffre.Expire : src.Statut
             ));

        }
    }
}
