using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ITipoDocumentoService
    {
        Task<GetTipoDocumentoDto> CrearTipoDocumentoAsync(Guid usuarioId, TipoDocumentoDto dto);
        Task ActuslizarTipoDocumentoAsync(Guid id, Guid usuarioId, ActualizarTipoDocumentoDto dto);
        Task<IEnumerable<GetTipoDocumentoDto>> ObtenerTodosAsync();
    }
}
