using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IMovimientoService
    {
        Task ActualizarMovimientoAsync(ActualizarMovimientoDto dto, Guid usuarioModificacionId, Guid idMovimiento);
        Task RegistrarMovimiento(RegistrarMovimientoDto dto, Guid usuarioCreacionId);
    }
}
