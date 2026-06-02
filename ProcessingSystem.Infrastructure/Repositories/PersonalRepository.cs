using Microsoft.EntityFrameworkCore;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using ProcessingSystem.Infrastructure.Data;

namespace ProcessingSystem.Infrastructure.Repositories
{
    public class PersonalRepository : IPersonalRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarDatosPersonalAsync(Usuarios usuarios)
        {
            _context.Usuarios.Update(usuarios);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuarios> CrearPersonalAsync(Usuarios usuarios)
        {
            await _context.Usuarios.AddRangeAsync(usuarios);
            await _context.SaveChangesAsync();
            return usuarios;
        }

        public async Task<IEnumerable<Usuarios>> GetAll()
        {
            return await _context.Usuarios
                .OrderBy(u => u.EstaEliminado)
                .ToListAsync();
        }

        public async Task<Usuarios?> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(p => p.Id == id);    
        }

        public async Task<Usuarios?> ObtenerPorDni(string dni)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == dni);
        }
    }
}
