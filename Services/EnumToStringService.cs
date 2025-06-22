
using Synoptis.API.Enums;

namespace Synoptis.API.Services
{
    public class EnumToStringService
    {
        public string StatutAoEnumService(StatutAppelOffre statut)
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

        public string RoleUserEnumService(UserRole role)
        {
            return role switch
            {
                UserRole.ResponsableAgence => "Responsable d'agence",
                UserRole.Secretaire => "Secretaire",
                UserRole.ChargeAffaires => "Chargé(e) d'affaires",
                _ => "Inconnu"

            };
        }

        // Version statique pour config globale pour Mapster
        public static string RoleUserEnumServiceStatic(UserRole role)
        {
            return role switch
            {
                UserRole.ResponsableAgence => "Responsable d'agence",
                UserRole.Secretaire => "Secretaire",
                UserRole.ChargeAffaires => "Chargé(e) d'affaires",
                _ => "Inconnu"
            };
        }

        public static string StatutAoEnumServiceStatic(StatutAppelOffre statut)
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
    }
}

