using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProcessingSystem.Application.Services
{
    public class OficinaService : IOficinaService
    {
        private readonly IOficinaRepository _oficinaRepository;
        private readonly IMapper _mapper;
        private readonly IUsuarioContextService _usuarioContextService;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IPersonalRepository _personalRepository;

        public OficinaService(IOficinaRepository oficinaRepository, IMapper mapper, IUsuarioContextService usuarioContextService, 
            UserManager<IdentityUser<Guid>> userManager, IPersonalRepository personalRepository)
        {
            _oficinaRepository = oficinaRepository;
            _mapper = mapper;
            _usuarioContextService = usuarioContextService;
            _userManager = userManager;
            _personalRepository = personalRepository;
        }

        public async Task ActualizarOficinaAsync(Guid usuarioId, Guid oficinaId, ActualizarOficinaDto dto)
        {
            var (_, usuarioModificacionId) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "actualizar datos de una oficina");

            var oficinaExiste = await _oficinaRepository.GetByIdAsync(oficinaId);
            if(oficinaExiste == null)
            {
                throw new KeyNotFoundException("Oficina no encontrada");
            }

            if (dto.OficinaPadreId.HasValue)
            {
                var padreExiste = await _oficinaRepository.GetByIdAsync(dto.OficinaPadreId.Value);
                if (padreExiste == null)
                {
                    throw new KeyNotFoundException("La oficina superior especificada no existe");
                }
            }

            dto.Adapt(oficinaExiste);
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
                    throw new KeyNotFoundException("La oficina superior especificada no existe");
                }
            }

            var nuevaOficina = dto.Adapt<Oficina>();
            nuevaOficina.UsuarioCreacion = usuarioCreacionId;
            var result = await _oficinaRepository.CrearOficinaAsync(nuevaOficina);

            return result.Adapt<OficinaDto>();
        }

        public async Task<IEnumerable<GetOficinasDto>> GetOficinasAsync()
        {
            var listaOficinasDto = await _oficinaRepository.GetAllAsync();
            var result = listaOficinasDto.Adapt<List<GetOficinasDto>>();

            var oficinas = await _oficinaRepository.GetAllAsync();

            if (!result.Any()) return result;

            var todoElPersonal = await _personalRepository.GetAll();

            var emailDiccionario = await ObtenerDiccionarioEmailsAsync(todoElPersonal);

            var personalPorOficina = todoElPersonal
                .Where(p => p.OficinaId.HasValue)
                .GroupBy(p => p.OficinaId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var oficinasDiccionario = oficinas.ToDictionary(o => o.Id);

            foreach (var oficinaDto in result)
            {
                var oficinaActual = oficinasDiccionario[oficinaDto.Id];
                oficinaDto.OficinaReporte = oficinaActual.OficinaPadreId.HasValue &&
                    oficinasDiccionario.TryGetValue(oficinaActual.OficinaPadreId.Value, out var oficinaSuperior)
                        ? oficinaSuperior.Nombre
                        : "Sin oficina de reporte";

                if (personalPorOficina.TryGetValue(oficinaDto.Id, out var trabajadoresDeEstaOficina))
                {
                    oficinaDto.Trabajadores = trabajadoresDeEstaOficina
                        .Where(p => p.EstaEliminado == false)
                        .Select(p => new TrabajadoresBasicoDto
                    {
                        Id = p.Id,
                        NombreCompleto = $"{p.Nombre} {p.Apellidos}".Trim(),
                        Dni = p.Dni,
                        Email = p.UserId.HasValue && emailDiccionario.TryGetValue(p.UserId.Value, out var email)
                            ? email
                            : "Sin Correo"
                    }).ToList();
                }
            }
            return result;
        }

        private async Task<Dictionary<Guid, string>> ObtenerDiccionarioEmailsAsync(IEnumerable<Usuarios> personalList)
        {
            var userIds = personalList
                .Where(p => p.UserId.HasValue)
                .Select(p => p.UserId!.Value)
                .Distinct()
                .ToList();

            if (!userIds.Any()) return new Dictionary<Guid, string>();

            var listaEmails = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            return listaEmails.ToDictionary(u => u.Id, u => u.Email ?? "Sin Correo");
        }
    }
}
