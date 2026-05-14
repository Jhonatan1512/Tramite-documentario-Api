using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Entities
{
    public class DocumentoArchivo : BaseEntity
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public string Extension {  get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public long Tamanio { get; set; }
        public Guid ExpedienteId { get; set; }
        public virtual Expediente? Expediente { get; set; }
    }
}
