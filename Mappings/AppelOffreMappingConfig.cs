using Mapster;
using Synoptis.API.DTOs;
using Synoptis.API.Models;

namespace Synoptis.API.Mappings
{
    public class AppelOffreMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AppelOffreCreateDTO, AppelOffre>()
                .Map(dest => dest.DateLimite,
                     src => DateTime.SpecifyKind(src.DateLimite, DateTimeKind.Utc));
        }
    }
}
