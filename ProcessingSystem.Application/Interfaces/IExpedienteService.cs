using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IExpedienteService
    {
        Task<GetExpedienteDto> CrearExpedienteAsync(Guid creadorId, ExpedienteDto dto);
        Task ActualizarExpedienteAsync(Guid expedienteId, Guid usuarioId, ExpedienteDto dto);
        Task EliminarExpedienteService(Guid expedienteId, Guid usuarioId);
        Task<IEnumerable<GetAllExpedientesDto>> GetExpedienteListAsync(Guid usuarioId);
        Task<IEnumerable<GetAllExpedientesDto>> ObtenerExpidientesPorPerfil();
    }
}
