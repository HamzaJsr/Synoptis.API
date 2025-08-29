using Synoptis.API.Data;

namespace Synoptis.API.Services.Abstractions
{
    /// <summary>
    /// Service de base "conscient du tenant".
    /// - Récupère le companyId depuis le JWT de l'utilisateur courant.
    /// - Expose CompanyId (Guid) à tes services métiers pour filtrer/injecter automatiquement.
    /// ⚠️ À utiliser SEULEMENT sur des routes protégées [Authorize] (donc avec un JWT).
    /// </summary>
    public abstract class TenantAwareService
    {
        // DbContext EF Core, accessible aux services dérivés (AppelOffreService, etc.)


        // Le companyId extrait du token (claim "companyId"), parsé en Guid.
        protected readonly Guid CompanyId;

        /// <summary>
        /// Le constructeur est appelé par l’IoC (DI).
        /// Il reçoit:
        ///  - le DbContext de la requête,
        ///  - un IHttpContextAccessor pour accéder au HttpContext (donc au User et à ses claims).
        /// </summary>
        protected TenantAwareService(IHttpContextAccessor http)
        {


            // On va chercher le claim "companyId" dans le JWT de l'utilisateur courant.
            // http.HttpContext                → le contexte HTTP en cours (peut être null hors requête HTTP).
            // ?.User                          → l'identité de l'utilisateur (ClaimsPrincipal).
            // .FindFirst("companyId")         → récupère le claim dont le type est "companyId".
            // ?.Value                         → prend la valeur du claim (un string de type Guid).
            // Si l'un des maillons est null (pas de contexte, pas d'utilisateur, pas de claim),
            // on lève une UnauthorizedAccessException pour signaler l'absence du tenant.
            var claim = http.HttpContext?.User.FindFirst("companyId")?.Value
                ?? throw new UnauthorizedAccessException("companyId manquant dans le token.");

            // On convertit la string (ex: "c0a8012e-...") en Guid utilisable dans les requêtes EF.
            CompanyId = Guid.Parse(claim);
        }
    }
}
