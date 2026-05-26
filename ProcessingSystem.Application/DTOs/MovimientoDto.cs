using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class MovimientoDto
    {
        public Guid OficinaOrigenId { get; set; }
        public Guid OficinaDestinoId { get; set; }
        public Guid EmisorId { get; set; }
        public string ComentarioDerivacion { get; set; } = string.Empty;
        public EstadoMovimiento Estado { get; set; }
    }

    public class ActualizarMovimientoDto
    {
        public DateTime FechaRecepcion { get; set; } = DateTime.Now;
        public string ComentarioFinal {  get; set; } = string.Empty;
    }

    public class RegistrarMovimientoDto 
    {
        public Guid ExpedienteId { get; set; }
        public Guid OficinaDestinoId { get; set; }
        public string? ComentarioDerivacion { get; set; } = string.Empty;
    }
}
