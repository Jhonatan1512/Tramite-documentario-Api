using Mapster;
using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class ExpedienteService : IExpedienteService
    {
        private readonly IExpedienteRepository _expedienteRepository;
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
        private readonly IUsuarioContextService _usuarioContextService;
        private readonly IOficinaRepository _oficinaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUsuarioRepository _usuarioRepository;

        public ExpedienteService(IExpedienteRepository expedienteRepository,              
            ITipoDocumentoRepository tipoDocumentoRepository, IUsuarioContextService usuarioContextService,
            IOficinaRepository oficinaRepository, ICurrentUserService currentUserService, IUsuarioRepository usuarioRepository)
        {
            _expedienteRepository = expedienteRepository;
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _usuarioContextService = usuarioContextService;
            _oficinaRepository = oficinaRepository;
            _currentUserService = currentUserService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task ActualizarExpedienteAsync(Guid expedienteId, Guid usuarioId, ExpedienteDto dto)
        {
            var (_, usuarioActualizacionId) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "actualizar un expediente");

            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(expedienteId);
            if(expedienteExiste == null)
            {
                throw new Exception("El registro a actualizar no existe");
            }

            expedienteExiste.Asunto = dto.Asunto;
            expedienteExiste.TipoDocumentoId = dto.TipoDocumentoId;
            expedienteExiste.FechaModificacion = DateTime.Now;
            expedienteExiste.UsuarioModificacion = usuarioActualizacionId;

            await _expedienteRepository.ActualizarExpedienteAsync(expedienteExiste);
        }

        public async Task<GetExpedienteDto> CrearExpedienteAsync(Guid creadorId, ExpedienteDto dto)
        {
            var (usuarioNegocio, usuarioCreacionId) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(creadorId, "crear un expediente");

            var esMesaDePartes = await _oficinaRepository.ObtenerMesaDePartesAsync();

            var nuevoExpediente = dto.Adapt<Expediente>();
            nuevoExpediente.UsuarioId = usuarioNegocio.Id;
            nuevoExpediente.Estado = EstadoExpediente.Registrado;
            nuevoExpediente.UsuarioCreacion = usuarioCreacionId;
            nuevoExpediente.OficinaId = esMesaDePartes!.Id;

            int totalExpedientes = await _expedienteRepository.ContarExpedientesPorUsuario();
            int numeroSiguiente = totalExpedientes + 1;
            nuevoExpediente.NumeroExpediente = $"EXP-{DateTime.Now.Year}-{numeroSiguiente.ToString("D4")}";

            var primerMovimiento = new MovimientoDto
            {
                OficinaOrigenId = esMesaDePartes.Id, 
                OficinaDestinoId = esMesaDePartes.Id,
                EmisorId = Guid.Parse(usuarioCreacionId),
                ComentarioDerivacion = "El expediente se registro en mesa de partes",
                Estado = EstadoMovimiento.Pendiente
            };

            var movimiento = primerMovimiento.Adapt<Movimiento>();
            nuevoExpediente.Historial.Add(movimiento);
            movimiento.UsuarioCreacion = usuarioCreacionId;

            var result = await _expedienteRepository.CrearExpedienteAsync(nuevoExpediente);
            return result.Adapt<GetExpedienteDto>();
        }

        public async Task EliminarExpedienteService(Guid expedienteId, Guid usuarioId)
        {
            var (usuarioNegocio, _) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "eliminar este registro");

            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(expedienteId);
            if(expedienteExiste == null)
            {
                throw new Exception("El expediente que esta intentando eliminar no existe");
            }

            if(expedienteExiste.UsuarioCreacion != usuarioNegocio.Id.ToString())
            {
                throw new Exception("No tiene las credenciales para eliminar este registro");
            }

            await _expedienteRepository.EliminarExpedienteAsync(expedienteId);
        }

        public async Task<IEnumerable<GetAllExpedientesDto>> GetExpedienteListAsync(Guid usuarioId)
        {
            var (usuarioNegocio, _) = await _usuarioContextService.ObtenerYValidarUsuarioAsync(usuarioId, "consultar los expedientes");

            var resultDto = await _expedienteRepository.ObtenerTodoslosExpedientesAsync(usuarioNegocio.Id);

            var result = resultDto.Adapt<List<GetAllExpedientesDto>>();

            foreach (var registro in result.Where(r => r.TipoDocumentoId != Guid.Empty))
            {
                var tipoDocDb = await _tipoDocumentoRepository.GetTipodocumntoByIdAsync(registro.TipoDocumentoId);
                registro.TipoDocumentoNombre = tipoDocDb != null ? tipoDocDb.Nombre : "Sin Tipo";
            }
            return result;
        }

        public async Task<IEnumerable<GetAllExpedientesDto>> ObtenerExpidientesPorPerfil()
        {
            var oficinaId = _currentUserService.GetOficinaId();
            var mesaPartes = await _oficinaRepository.ObtenerMesaDePartesAsync();

            List<GetAllExpedientesDto> resultDto;

            if (mesaPartes != null && oficinaId == mesaPartes.Id)
            {
                var expedientesMesa = await _expedienteRepository.ObtenerExpedientesMesaPartesAsync();
                resultDto = expedientesMesa.Adapt<List<GetAllExpedientesDto>>();
            }
            else
            {
                var result = await _expedienteRepository.ObtenerPorOficinaAsync(oficinaId);
                resultDto = result.Adapt<List<GetAllExpedientesDto>>();
            }

            var usuarioIds = resultDto
                .Where(u => u.UsuarioCreacion != Guid.Empty)
                .Select(u => u.UsuarioCreacion)
                .Distinct()
                .ToList();

            if (usuarioIds.Any())
            {
                var listaUsuarios = await _usuarioRepository.ObtenerUsuariosPorListaIdsAsync(usuarioIds);

                foreach (var item in resultDto)
                {
                    var usuarioDb = listaUsuarios.FirstOrDefault(u => u.Id == item.UsuarioCreacion);
                    if (usuarioDb != null)
                    {
                        item.NombreUsuarioCreacion = $"{usuarioDb.Nombre} {usuarioDb.Apellidos}";
                        item.DniUsuarioCreacion = usuarioDb.Dni;
                    }
                    else
                    {
                        item.NombreUsuarioCreacion = "Usuario no encontrado";
                        item.DniUsuarioCreacion = "Sin Dni";
                    }
                }
            }
            return resultDto;
        }
    }
}
