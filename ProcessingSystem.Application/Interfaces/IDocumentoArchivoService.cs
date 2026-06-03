using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IDocumentoArchivoService
    {
        Task<DocumentoArchivoDto> SubirArchivoAsync(Guid usuarioId, SubirArchivoDto dto);
        Task EliminarArchivoAsync(Guid archivoId, Guid usuarioId);
        Task ActualizarArchivoAsync(Guid usuarioId, ActualizarArchivoDto dto);
        Task<GetArchivoDto?> ObtenerArchivAsync(Guid archivoId, string rutaBaseWeb, Guid usuarioId);
    }
}
