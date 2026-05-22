using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IOficinaService
    {
        Task<OficinaDto> CrearOficinaAsync(Guid usuarioId, OficinaDto dto);
        Task ActualizarOficinaAsync(Guid usuarioId, Guid oficinaId, ActualizarOficinaDto dto);
        Task<IEnumerable<GetOficinasDto>> GetOficinasAsync();
    }
}
