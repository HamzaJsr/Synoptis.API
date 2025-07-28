using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synoptis.API.DTOs
{
    public class AppelOffreDocumentDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string NomFichier { get; set; } = string.Empty;
        public string TypeDocument { get; set; } = string.Empty; // Ex: "CCTP", "RC", "DPGF", etc.

        public DateTime DateDepot { get; set; } = DateTime.UtcNow;

        public string Url { get; internal set; } = string.Empty;
    }
}