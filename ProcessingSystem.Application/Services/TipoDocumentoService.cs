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
        private readonly IUsuarioContextService _usuarioContext;

        public TipoDocumentoService(ITipoDocumentoRepository tipoDocumentoRepository, IUsuarioRepository usuarioRepository, 
            UserManager<IdentityUser<Guid>> userManager, IUsuarioContextService usuarioContext)
        {
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _usuarioContext = usuarioContext;
        }

        public async Task ActuslizarTipoDocumentoAsync(Guid id, Guid usuarioId, ActualizarTipoDocumentoDto dto)
        {
            var (_, usuarioModificacionId) = await _usuarioContext.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento");

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
            var (_, usuarioCreacionId) = await _usuarioContext.ObtenerYValidarUsuarioAsync(usuarioId, "crear un tipo de documento");

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
                var (usuarioCreacion, _) = await _usuarioContext.ObtenerYValidarUsuarioAsync(Guid.Parse(dto.UsuarioCreacion), "ver la lista");
                dto.NombreUsuarioCreacion = "Administrador del Distema";
            }
            return result;
        }
    }
}
