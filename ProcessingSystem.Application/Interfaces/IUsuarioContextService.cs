using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IUsuarioContextService
    {
        Task<(Usuarios UsuarioNegocio, string AuditUsuario)> ObtenerYValidarUsuarioAsync(Guid usuarioId, string accionMensaje);
    }
}
