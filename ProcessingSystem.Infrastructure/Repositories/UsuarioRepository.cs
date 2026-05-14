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

        public async Task<Usuarios> CrearUsuarioAsync(Usuarios usuario)
        {
            
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;

        }

        public async Task<IEnumerable<Usuarios>> GetAll() 
        {
            var data = new List<Usuarios>
            {
                new Usuarios {Id = Guid.NewGuid(), Nombre = "Jhonatan", Apellidos = "Cruzado", Dni = "12345678"},
                new Usuarios { Id = Guid.NewGuid(), Nombre = "Jhoselin", Apellidos = "Cano", Dni = "14785236" },
            };

            return await Task.FromResult<IEnumerable<Usuarios>>(data);
        }

        public async Task<Usuarios?> ObtenerPorDniAsync(string dni)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == dni);
        }
    }
}
