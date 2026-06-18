using Microsoft.AspNetCore.Identity;
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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarUsuarioAsync(Usuarios usuarios)
        {
            _context.Usuarios.Update(usuarios);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuarios> CrearUsuarioAsync(Usuarios usuario)
        {            
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<string> ObtenerOficinaPersonalAsync(Guid usuarioId)
        {            
            var oficinaId = await _context.Usuarios
                .Where(u => u.UserId == usuarioId)
                .Select(u => (Guid?)u.OficinaId)
                .FirstOrDefaultAsync();

            if(oficinaId == null)
            {
                throw new InvalidOperationException("Si oficina");
            }
            return oficinaId.Value.ToString();
        }

        public async Task<Usuarios?> ObtenerPorDniAsync(string dni)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == dni);
        }

        public async Task<Usuarios?> ObtenerPorId(Guid usuarioId)
        {
            return await _context.Usuarios
                .Include(u => u.Oficina)
                .FirstOrDefaultAsync(u => u.UserId == usuarioId);
        }

        public async Task<IEnumerable<Usuarios>> ObtenerUsuariosPorListaIdsAsync(List<Guid> ids)
        {
            return await _context.Usuarios
                .Where(u => ids.Contains(u.Id) && u.EstaEliminado == false)
                .ToListAsync();
        }
    }
}
