using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface IPersonalRepository
    {
        Task<Usuarios> CrearPersonalAsync(Usuarios usuarios);
        Task<Usuarios?> ObtenerPorDni(string dni);
    }
}
