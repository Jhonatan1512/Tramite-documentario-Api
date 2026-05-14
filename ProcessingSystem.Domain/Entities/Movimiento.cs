using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Entities
{
    public class Movimiento : BaseEntity
    {
        public Guid ExpedienteId {  get; set; }
        public virtual Expediente? Expediente { get; set; }
        public Guid OficinaOrigenId { get; set; }
        public virtual Oficina? OficinaOrigen { get; set; }
        public Guid OficinaDestinoId { get; set; }
        public virtual Oficina? OficinaDestino { get; set; }
        public Guid EmisorId { get; set; }
        public Guid? ReceptorId { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string ComentarioDerivacion { get; set; } = string.Empty;
        public string? ComentarioFinal {  get; set; }
        public  EstadoMovimiento Estado { get; set; }
    }

    public enum EstadoMovimiento
    {
        Pendiente,
        Recibido, 
        Derivado, 
        Atendido, 
        Rechazado
    }
}
