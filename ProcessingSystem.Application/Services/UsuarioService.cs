using Mapster;
using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICredencialesCiudadanosService _ciudadanosService;
        private readonly IIdentityCiudadanoiService _identityCiudadano;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public UsuarioService(IUsuarioRepository usuarioRepository, ICredencialesCiudadanosService ciudadanosService, 
            IIdentityCiudadanoiService identityCiudadano, UserManager<IdentityUser<Guid>> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _ciudadanosService = ciudadanosService;
            _identityCiudadano = identityCiudadano;
            _userManager = userManager;
        }
        
        public async Task ActualizarContrasenaAsync(Guid id, ActualizarContrasenaDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if(usuario == null)
            {
                throw new KeyNotFoundException("El usuario no existe");
            }

            await _identityCiudadano.ActualizarContrasenaAsync(id, dto.ContrasenaActual, dto.ContrasenaNueva);
        }

        public async Task ActualizarUsuarioAsync(Guid id, ActualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
            {
                throw new KeyNotFoundException("El usuario no existe");
            }

            await _ciudadanosService.VerificarDni(usuario.Dni, dto.Dni);

            dto.Adapt(usuario);
            usuario.UsuarioModificacion = usuario.Id.ToString();
            usuario.FechaModificacion = DateTime.Now;

            await _usuarioRepository.ActualizarUsuarioAsync(usuario);            
        }

        public async Task<GetUsuarioDto> CrearUsuarioAsync(UsuariosDto dto) 
        {
            await _ciudadanosService.ValidarRegistrosDuplicados(dto.Email, dto.Dni);

            var identityUserId = await _identityCiudadano.CrearCuentaAsync(dto.Email, dto.Password);
            await _identityCiudadano.AsignarRoleAsync(identityUserId, "Ciudadano");     

            var usuarioDto = dto.Adapt<Usuarios>();
            usuarioDto.UserId = identityUserId;

            await _usuarioRepository.CrearUsuarioAsync(usuarioDto);

            var result = usuarioDto.Adapt<GetUsuarioDto>();
            result.Email = dto.Email;
            return result;
        }

        public async Task<GetPerfilDto> ObtenerPerfilAsync(Guid id)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerPorId(id);
            if (usuarioExiste == null) { throw new KeyNotFoundException("El usuario no existe"); }

            var usuarioIdentity = await _userManager.FindByIdAsync(id.ToString());
            if (usuarioIdentity == null)
            {
                throw new KeyNotFoundException("Usuario de seguridad no encontrado");
            }

            var result = usuarioExiste.Adapt<GetPerfilDto>();
            result.Oficina = usuarioExiste.Oficina != null ? usuarioExiste.Oficina.Nombre : "Ciudadano";
            result.Email = usuarioIdentity.Email!;
            return result;
        }
    }
}
