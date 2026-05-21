using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface IDocumentoArchivoRepository
    {
        Task<DocumentoArchivo> CrearDocumentoArchivoAsync(DocumentoArchivo documentoArchivo);
        Task<DocumentoArchivo?> ObtenerArchivoPorIdAsync(Guid id);
        Task<string?> EliminarArchivo(Guid archivoId);
        Task ActualizarArchivoAsync();
    }
}
