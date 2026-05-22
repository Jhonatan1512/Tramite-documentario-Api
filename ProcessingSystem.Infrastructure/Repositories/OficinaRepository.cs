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
    public class OficinaRepository : IOficinaRepository
    {
        private readonly ApplicationDbContext _context;

        public OficinaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarOficina(Oficina oficina)
        {
            _context.Oficinas.Update(oficina);
            await _context.SaveChangesAsync();
        }

        public async Task<Oficina> CrearOficinaAsync(Oficina oficina)
        {
            await _context.Oficinas.AddAsync(oficina);
            await _context.SaveChangesAsync();
            return oficina;
        }

        public async Task<IEnumerable<Oficina>> GetAllAsync()
        {
            return await _context.Oficinas
                .Include(o => o.Trabajadores)
                .ToListAsync();
        }

        public async Task<Oficina?> GetByIdAsync(Guid id)
        {
            return await _context.Oficinas.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Oficina?> ObtenerMesaDePartesAsync()
        {
            return await _context.Oficinas.FirstOrDefaultAsync(o => o.EsMesaPartes);
        }
    }
}
