using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class ExpedienteDto
    {
        public string Asunto {  get; set; } = string.Empty;
        public Guid TipoDocumentoId { get; set; }
    }

    public class GetExpedienteDto
    {
        public Guid Id { get; set; }
        public string NumeroExpediente { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string TipoDocumentoNombre {  get; set; } = string.Empty;
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
        public DateTime FechaCreacion {  get; set; }
    }
}
