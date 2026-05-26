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
    public class MovimientoRepository : IMovimientoRepository
    {
        private readonly ApplicationDbContext _context;

        public MovimientoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarMovimientoAsync(Movimiento movimiento)
        {
            _context.Movimientos.Update(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task<Movimiento> CrearMovimientoAsync(Movimiento movimiento)
        {
            await _context.Movimientos.AddAsync(movimiento);
            await _context.SaveChangesAsync();
            return movimiento;
        }

        public async Task<IEnumerable<Movimiento>> ObtenerMovimientosAsync(Guid expedienteId)
        {
            return await _context.Movimientos.Where(m => m.ExpedienteId == expedienteId).ToListAsync();
        }

        public async Task<Movimiento?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Movimientos.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
