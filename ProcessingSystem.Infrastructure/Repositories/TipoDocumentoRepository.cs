using Mapster;
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
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly ApplicationDbContext _context;

        public TipoDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ActualizarTipoDocumentoAsync(TipoDocumento documento)
        {
            _context.TipoDocumentos.Update(documento);
            await _context.SaveChangesAsync();
        }

        public async Task<TipoDocumento> CrearTipoDocumentoAsync(TipoDocumento documento)
        {
            await _context.TipoDocumentos.AddAsync(documento);
            await _context.SaveChangesAsync();
            return documento;

        }

        public async Task<IEnumerable<TipoDocumento>> GetAllTiposDocumentoAsync()
        {
            return await _context.TipoDocumentos
                .Where(t => !t.EstaEliminado)
                .ProjectToType<TipoDocumento>()
                .ToListAsync();
        }

        public async Task<TipoDocumento?> GetTipodocumntoByIdAsync(Guid id)
        {
            return await _context.TipoDocumentos.FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
