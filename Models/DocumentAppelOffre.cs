using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synoptis.API.Models
{
    public class DocumentAppelOffre
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string NomFichier { get; set; } = string.Empty;
        public string TypeDocument { get; set; } = string.Empty; // Ex: "CCTP", "RC", "DPGF", etc.

        public DateTime DateDepot { get; set; } = DateTime.UtcNow;

        // 🔗 Relations
        public Guid AppelOffreId { get; set; }
        public AppelOffre AppelOffre { get; set; } = null!;

        public Guid DeposeParId { get; set; }
        public User DeposePar { get; set; } = null!;
    }
}