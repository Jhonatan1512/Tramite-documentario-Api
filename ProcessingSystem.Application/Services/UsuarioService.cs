using Mapster;
using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.DTOs;
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
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICredencialesCiudadanosService _ciudadanosService;
        private readonly IIdentityCiudadanoiService _identityCiudadano;

        public UsuarioService(IUsuarioRepository usuarioRepository, ICredencialesCiudadanosService ciudadanosService, IIdentityCiudadanoiService identityCiudadano)
        {
            _usuarioRepository = usuarioRepository;
            _ciudadanosService = ciudadanosService;
            _identityCiudadano = identityCiudadano;
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
    }
}
