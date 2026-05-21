using Mapster;
using MapsterMapper;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class OficinaService : IOficinaService
    {
        private readonly IOficinaRepository _oficinaRepository;
        private readonly IMapper _mapper;
        private readonly IUsuarioContextService _usuarioContextService;

        public OficinaService(IOficinaRepository oficinaRepository, IMapper mapper, IUsuarioContextService usuarioContextService)
        {
            _oficinaRepository = oficinaRepository;
            _mapper = mapper;
            _usuarioContextService = usuarioContextService;
        }

        public async Task ActualizarOficinaAsync(Guid usuarioId, Guid oficinaId, OficinaDto dto)
        {
            var (_, usuarioModificacionId) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "actualizar datos de una oficina");

            var oficinaExiste = await _oficinaRepository.GetByIdAsync(oficinaId);
            if(oficinaExiste == null)
            {
                throw new KeyNotFoundException("Oficina no encontrada");
            }

            oficinaExiste.Nombre = dto.Nombre;
            oficinaExiste.UsuarioModificacion = usuarioModificacionId;
            oficinaExiste.FechaModificacion = DateTime.Now;

            await _oficinaRepository.ActualizarOficina(oficinaExiste);
        }

        public async Task<OficinaDto> CrearOficinaAsync(Guid usuarioId, OficinaDto dto)
        {
            var (_, usuarioCreacionId) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "crear una oficina");

            if (dto.OficinaPadreId.HasValue)
            {
                var padreExiste = await _oficinaRepository.GetByIdAsync(dto.OficinaPadreId.Value);
                if (padreExiste == null)
                {
                    throw new Exception("La oficina superior especificada no existe");
                }
            }

            var nuevaOficina = dto.Adapt<Oficina>();
            nuevaOficina.UsuarioCreacion = usuarioCreacionId;

            var result = await _oficinaRepository.CrearOficinaAsync(nuevaOficina);

            return result.Adapt<OficinaDto>();
        }
    }
}
