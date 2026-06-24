using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IIdentityCiudadanoiService
    {
        Task<Guid> CrearCuentaAsync(string email, string password);
        Task AsignarRoleAsync(Guid identityRole, string nombreRole);
        Task ActualizarContrasenaAsync(Guid id, string contrasenaActual, string contrsenaNueva);
    }
}
