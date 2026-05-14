using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Entities
{
    public class Expediente : BaseEntity
    {
        public string NumeroExpediente { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public virtual Usuarios? Usuario { get; set; }
        public Guid TipoDocumentoId { get; set; }
        public virtual TipoDocumento? TipoDocumento { get; set; }
        public EstadoExpediente? Estado {  get; set; }
        public virtual ICollection<DocumentoArchivo> Archivos { get; set; } = [];
        public virtual ICollection<Movimiento> Historial { get; set; } = [];
    }

    public enum EstadoExpediente
    {
        Registrado, 
        EnProceso, 
        Finalizado, 
        Observado
    }
}
