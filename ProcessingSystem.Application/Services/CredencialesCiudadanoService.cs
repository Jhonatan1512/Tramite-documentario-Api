using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class CredencialesCiudadanoService : ICredencialesCiudadanosService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public CredencialesCiudadanoService(IUsuarioRepository usuarioRepository, UserManager<IdentityUser<Guid>> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        public async Task ValidarRegistrosDuplicados(string email, string dni)
        {
            var dniExiste = await _usuarioRepository.ObtenerPorDniAsync(dni);
            if(dniExiste != null)
            {
                throw new InvalidOperationException("El DNI ya esta registrado");
            }

            var exiteEmail = await _userManager.FindByEmailAsync(email);
            if(exiteEmail != null)
            {
                throw new InvalidOperationException("El email ya esta registrado");
            }
        }

        public async Task VerificarDni(string dniActual, string dniNuevo)
        {
            if (dniActual == dniNuevo) return;

            var dniExiste = await _usuarioRepository.ObtenerPorDniAsync(dniNuevo);
            if (dniExiste != null)
                throw new InvalidOperationException("El DNI ya pertenece a otro usuario");
        }
    }
}
