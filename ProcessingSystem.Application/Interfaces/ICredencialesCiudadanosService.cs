using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ICredencialesCiudadanosService
    {
        Task ValidarRegistrosDuplicados(string email, string dni);
        Task VerificarDni(string dniActual, string dniNuevo);
    }
}
