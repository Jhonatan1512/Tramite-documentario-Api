using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProcessingSystem.Application.DTOs
{
    public class DocumentoArchivoDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Extension {  get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public long Tamanio { get; set; }
        public Guid ExpedienteId { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    public class SubirArchivoDto
    {
        public Guid ExpedienteId { get; set; }
        public IFormFile Archivo { get; set; } = null!;
    }

    public class ActualizarArchivoDto
    {
        public Guid Id {  set; get; }
        public IFormFile Archivo { get; set; } = null!;
    }

    public class GetArchivoDto
    {
        public Stream Contenido { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string NombreArchivo { get; set; } = null!;
    }
}
