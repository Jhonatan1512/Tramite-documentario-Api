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
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public TipoDocumentoService(ITipoDocumentoRepository tipoDocumentoRepository, IUsuarioRepository usuarioRepository, UserManager<IdentityUser<Guid>> userManager)
        {
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        public async Task ActuslizarTipoDocumentoAsync(Guid id, Guid usuarioId, ActualizarTipoDocumentoDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(usuarioId);

            string usuarioModificacionId;
            if(usuario != null)
            {
                usuarioModificacionId = usuario.Id.ToString();
            } else
            {
                var userIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if(userIdentity == null)
                {
                    throw new Exception("El usuario no existe en el sistema");
                } 

                usuarioModificacionId = userIdentity.Id.ToString();
            }

            var registroExiste = await _tipoDocumentoRepository.GetTipodocumntoByIdAsync(id);
            if(registroExiste == null)
            {
                throw new Exception("El registro no existe");
            }

            registroExiste.Nombre = dto.Nombre;
            registroExiste.Descripcion = dto.Descripcion;
            registroExiste.FechaModificacion = dto.FechaActualizacion;
            registroExiste.UsuarioModificacion = usuarioModificacionId;

            await _tipoDocumentoRepository.ActualizarTipoDocumentoAsync(registroExiste);
        }

        public async Task<GetTipoDocumentoDto> CrearTipoDocumentoAsync(Guid usuarioId, TipoDocumentoDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(usuarioId);

            string usuarioCreacionId;
            if (usuario != null)
            {
                usuarioCreacionId = usuario.Id.ToString();
            } else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if(usuarioIdentity == null)
                {
                    throw new Exception("El usuario no existe en el sistema");
                }
                usuarioCreacionId = usuarioIdentity.Id.ToString();
            }

            var nuevoTipoDocumento = dto.Adapt<TipoDocumento>();
            nuevoTipoDocumento.UsuarioCreacion = usuarioCreacionId;

            var result = await _tipoDocumentoRepository.CrearTipoDocumentoAsync(nuevoTipoDocumento);

            return result.Adapt<GetTipoDocumentoDto>();
        }

        public async Task<IEnumerable<GetTipoDocumentoDto>> ObtenerTodosAsync()
        {
            var datosDto = await _tipoDocumentoRepository.GetAllTiposDocumentoAsync();
            
            var result = datosDto.Adapt<List<GetTipoDocumentoDto>>();

            foreach (var dto in result)
            {
                if(!string.IsNullOrWhiteSpace(dto.UsuarioCreacion) && Guid.TryParse(dto.UsuarioCreacion, out var userId))
                {
                    var usuarioDb = await _usuarioRepository.ObtenerPorId(userId);
                    if(usuarioDb != null)
                    {
                        dto.NombreUsuarioCreacion = $"{usuarioDb.Nombre} {usuarioDb.Apellidos}";
                        continue;
                    }

                    var usuarioIdentity = await _userManager.FindByIdAsync(dto.UsuarioCreacion);
                    if( usuarioIdentity != null)
                    {
                        dto.NombreUsuarioCreacion = "Administrador del Distema";
                    } else
                    {
                        dto.NombreUsuarioCreacion = "Usuario Desconocido";
                    }
                }
            }

            return result;
        }
    }
}
