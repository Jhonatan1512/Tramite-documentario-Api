using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class ExpedienteDto
    {
        public string Asunto { get; set; } = string.Empty;
        public Guid TipoDocumentoId { get; set; }
    }

    public class GetExpedienteDto
    {
        public Guid Id { get; set; }
        public string NumeroExpediente { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string TipoDocumentoNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
    }

    public class GetAllExpedientesDto
    {
        public Guid Id { get; set; }
        public string NumeroExpediente { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public Guid TipoDocumentoId { get; set; }
        public string TipoDocumentoNombre { get; set; } = string.Empty;
        public Guid UsuarioCreacion {  get; set; } 
        public string NombreUsuarioCreacion { get; set; } = string.Empty;
        public string DniUsuarioCreacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public IEnumerable<ArchivoDto> Archivos { get; set; } = [];
        public IEnumerable<HistorialMovimientosDto> Historial { get; set; } = [];
    }

    public class ArchivoDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    public class HistorialMovimientosDto
    {
        public string Estado { get; set; } = string.Empty;
        public string ComentarioDerivacion { get; set; } = string.Empty;
        public string? ComentarioFinal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public Guid OficinaDestinoId { get; set; }
        public Guid OficinaOrigenId { get; set; }
    }
}
