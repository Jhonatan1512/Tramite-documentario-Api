using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IPersonalService
    {
        Task<GetPersonalDto> CrearPersonalAsync(Guid usuarioCreacionId, PersonalDto dto);
        Task ActualizarPersonalAsync(Guid personalId, Guid usuarioId, ActualizarPersonalDto dto);
        Task<IEnumerable<GetPersonalDto>> ObtenerTodosAsync();
        Task<string> EliminarUsuario(Guid personalId, Guid usuarioId, DesactivarActivarPersonalDto dto);
    }
}
