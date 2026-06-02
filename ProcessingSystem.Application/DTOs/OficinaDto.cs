using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class OficinaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public Guid? OficinaPadreId { get; set; }
    }

    public class ActualizarOficinaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public Guid? OficinaPadreId { get; set; }
    }

    public class GetOficinasDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public Guid OficinaPadreId { get; set; }
        public string OficinaReporte { get; set; } = string.Empty;
        public IEnumerable<TrabajadoresBasicoDto> Trabajadores { get; set; } = [];
    }

    public class TrabajadoresBasicoDto
    {
        public Guid Id {  set; get; }
        public string NombreCompleto {  set; get; } = string.Empty;
        public string Dni {  set; get; } = string.Empty;
        public string Email {  set; get; } = string.Empty;
    }
}
