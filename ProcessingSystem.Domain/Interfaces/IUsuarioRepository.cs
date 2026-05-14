using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuarios>> GetAll();
        Task<Usuarios> CrearUsuarioAsync(Usuarios usuario);
        Task<Usuarios?> ObtenerPorDniAsync(string dni);
        Task<Usuarios?> ObtenerPorId(Guid usuarioId);
        Task ActualizarUsuarioAsync(Usuarios usuarios);
    }
}
