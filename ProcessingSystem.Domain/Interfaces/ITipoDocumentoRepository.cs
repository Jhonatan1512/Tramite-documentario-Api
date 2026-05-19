using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface ITipoDocumentoRepository
    {
        Task<TipoDocumento> CrearTipoDocumentoAsync(TipoDocumento documento);
        Task ActualizarTipoDocumentoAsync(TipoDocumento documento);
        Task<IEnumerable<TipoDocumento>> GetAllTiposDocumentoAsync();
        Task<TipoDocumento?> GetTipodocumntoByIdAsync(Guid id);
    }
}
