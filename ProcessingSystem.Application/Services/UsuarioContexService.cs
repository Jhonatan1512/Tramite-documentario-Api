using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class UsuarioContexService : IUsuarioContextService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public UsuarioContexService(IUsuarioRepository usuarioRepository, UserManager<IdentityUser<Guid>> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }
        public async Task<(Usuarios UsuarioNegocio, string AuditUsuario)> ObtenerYValidarUsuarioAsync(Guid usuarioId, string accionMensaje)
        {
            var usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioId);
            string auditUserId;

            if (usuarioNegocio != null)
            {
                auditUserId = usuarioNegocio.Id.ToString();
            }
            else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if (usuarioIdentity == null)
                {
                    throw new Exception($"El usuario no existe o no esta autorizado para {accionMensaje}");
                }

                auditUserId = usuarioIdentity.Id.ToString();
                usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioIdentity.Id);

                if (usuarioNegocio == null)
                {
                    throw new Exception($"para {accionMensaje}, el ciudadano debe estar activo");
                }
            }
            return (usuarioNegocio, auditUserId);
        }
    }
}
