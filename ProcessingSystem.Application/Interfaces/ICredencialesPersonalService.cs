using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ICredencialesPersonalService
    {
        (string Email, string Password) GenerarParaPersonal (string apellidos, string dni);
        Task ActualizarCredenciales(Guid identityUser, string apellidos, string dni);
    }
}
