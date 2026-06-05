using Microsoft.EntityFrameworkCore;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using ProcessingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Infrastructure.Repositories
{
    public class ExpedienteRepository : IExpedienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpedienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarEstado(Expediente expediente)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarExpedienteAsync(Expediente expediente)
        {
            _context.Update(expediente);
            await _context.SaveChangesAsync();
        }

        public async Task<Expediente?> BuscarExpedientePorIdAsync(Guid id)
        {
            return await _context.Expedientes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> ContarExpedientesPorUsuario()
        {
            return await _context.Expedientes.CountAsync();
        }

        public async Task<Expediente> CrearExpedienteAsync(Expediente expediente)
        {
            await _context.Expedientes.AddAsync(expediente);
            await _context.SaveChangesAsync();
            return expediente;
        }

        public async Task EliminarExpedienteAsync(Guid expedienteId)
        {
            await _context.Expedientes.Where(e => e.Id == expedienteId).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Expediente>> ObtenerExpedientesMesaPartesAsync()
        {
            return await _context.Expedientes
                .Include(e => e.Archivos)
                .Include(e => e.Historial)
                .OrderByDescending(e => e.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expediente>> ObtenerPorOficinaAsync(Guid oficinaId)
        {
            return await _context.Expedientes
                .Include(e => e.Archivos)
                .Include(e => e.Historial)
                .Where(e => _context.Movimientos
                    .Where(m => m.ExpedienteId == e.Id)
                    .OrderByDescending(m => m.FechaCreacion)
                    .Select(m => m.OficinaDestinoId)
                    .FirstOrDefault() == oficinaId)
                .OrderByDescending(e => e.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expediente>> ObtenerTodoslosExpedientesAsync(Guid usuarioId)
        {
            return await _context.Expedientes
                .Include(e => e.Archivos)
                .Include(e => e.TipoDocumento)
                .Include(e => e.Historial)
                .Where(e => e.UsuarioCreacion == usuarioId.ToString())
                .ToListAsync();
        }
    }
}
