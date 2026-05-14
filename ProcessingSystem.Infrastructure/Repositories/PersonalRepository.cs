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
    public class PersonalRepository : IPersonalRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonalRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Usuarios> CrearPersonalAsync(Usuarios usuarios)
        {
            await _context.Usuarios.AddRangeAsync(usuarios);
            await _context.SaveChangesAsync();
            return usuarios;
        }

        public async Task<Usuarios?> ObtenerPorDni(string dni)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == dni);
        }
    }
}
