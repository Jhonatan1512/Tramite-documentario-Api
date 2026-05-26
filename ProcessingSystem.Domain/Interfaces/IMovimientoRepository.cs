using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{ 
    public interface IMovimientoRepository
    {
        Task<Movimiento> CrearMovimientoAsync(Movimiento movimiento);
        Task<IEnumerable<Movimiento>> ObtenerMovimientosAsync(Guid documentoId);
        Task<Movimiento?> ObtenerPorIdAsync(Guid id);
        Task ActualizarMovimientoAsync(Movimiento movimiento);
    }
}
