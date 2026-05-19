using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class TipoDocumentoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
    public class GetTipoDocumentoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string NombreUsuarioCreacion { get; set; } = string.Empty;
    }

    public class ActualizarTipoDocumentoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaActualizacion {  get; set; } = DateTime.Now;
    }
}
