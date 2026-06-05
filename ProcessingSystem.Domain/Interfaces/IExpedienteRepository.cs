using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface IExpedienteRepository
    {
        Task<Expediente> CrearExpedienteAsync(Expediente expediente);
        Task<Expediente?> BuscarExpedientePorIdAsync(Guid id);
        Task<IEnumerable<Expediente>> ObtenerTodoslosExpedientesAsync(Guid usuarioId);
        Task<int> ContarExpedientesPorUsuario();
        Task ActualizarExpedienteAsync(Expediente expediente);
        Task EliminarExpedienteAsync(Guid expedienteId);
        Task ActualizarEstado(Expediente expediente);
        Task<IEnumerable<Expediente>> ObtenerExpedientesMesaPartesAsync();
        Task<IEnumerable<Expediente>> ObtenerPorOficinaAsync(Guid oficinaId); 
    }
}
