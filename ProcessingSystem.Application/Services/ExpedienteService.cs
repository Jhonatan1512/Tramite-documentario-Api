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
    public class ExpedienteService : IExpedienteService
    {
        private readonly IExpedienteRepository _expedienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;

        public ExpedienteService(IExpedienteRepository expedienteRepository, 
            IUsuarioRepository usuarioRepository, UserManager<IdentityUser<Guid>> userManager, ITipoDocumentoRepository tipoDocumentoRepository)
        {
            _expedienteRepository = expedienteRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _tipoDocumentoRepository = tipoDocumentoRepository;
        }

        public async Task ActualizarExpedienteAsync(Guid expedienteId, Guid usuarioId, ExpedienteDto dto)
        {
            var usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioId);
            string usuarioActualizacionId;
            if(usuarioNegocio != null)
            {
                usuarioActualizacionId = usuarioNegocio.Id.ToString();
            } else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if(usuarioIdentity == null)
                {
                    throw new Exception("el usuario no existe");
                }

                usuarioActualizacionId = usuarioIdentity.Id.ToString();

                usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioIdentity.Id);
                if (usuarioNegocio == null)
                {
                    throw new Exception("Para actualizar un expediente, el ciudadano debe estar  activo");
                }
            }

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
            var usuarioNegocio = await _usuarioRepository.ObtenerPorId(creadorId);
            string usuarioCreacionId;
            if (usuarioNegocio != null)
            {
                usuarioCreacionId = usuarioNegocio.Id.ToString();
            } else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(creadorId.ToString());
                if(usuarioIdentity == null)
                {
                    throw new Exception("el usuario no existe");
                }

                usuarioCreacionId = usuarioIdentity.Id.ToString();

                usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioIdentity.Id);
                if (usuarioNegocio == null)
                {
                    throw new Exception("Para crear un expediente, el ciudadano debe estar activo.");
                }
            }

            var nuevoExpediente = dto.Adapt<Expediente>();
            nuevoExpediente.UsuarioId = usuarioNegocio.Id;
            nuevoExpediente.Estado = EstadoExpediente.Registrado;
            nuevoExpediente.UsuarioCreacion = usuarioCreacionId;

            int numeroExpediente = (await _expedienteRepository.ObtenerTodoslosExpedientesAsync(creadorId)).Count() + 1;
            nuevoExpediente.NumeroExpediente = $"EXP-{DateTime.Now.Year}-{numeroExpediente.ToString("D4")}";

            var result = await _expedienteRepository.CrearExpedienteAsync(nuevoExpediente);

            return result.Adapt<GetExpedienteDto>();
        }

        public async Task EliminarExpedienteService(Guid expedienteId, Guid usuarioId)
        {
            var usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioId);
            string usucarioEliminacionId;
            if(usuarioNegocio != null)
            {
                usucarioEliminacionId = usuarioNegocio.Id.ToString();
            } else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if(usuarioIdentity == null)
                {
                    throw new Exception("No esta autorizado para eliminar este registro");
                }

                usucarioEliminacionId = usuarioIdentity.Id.ToString();

                usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioIdentity.Id);
                if(usuarioNegocio == null)
                {
                    throw new Exception("Para eliminar este registro, el ciudadano debe estar activo");
                }
            }

            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(expedienteId);
            if(expedienteExiste == null)
            {
                throw new Exception("El expediente que esta intentando eliminar no existe");
            }

            if(expedienteExiste.UsuarioCreacion != usuarioNegocio.Id.ToString())
            {
                throw new Exception("No tiene las credenciales para eliminar este registro");
            }

            await _expedienteRepository.EliminarExpedienteAsync(expedienteId, usuarioNegocio.Id);
        }

        public async Task<IEnumerable<GetAllExpedientesDto>> GetExpedienteListAsync(Guid usuarioId)
        {
            var usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioId);
            string usucarioConsultaId;
            if (usuarioNegocio != null)
            {
                usucarioConsultaId = usuarioNegocio.Id.ToString();
            }
            else
            {
                var usuarioIdentity = await _userManager.FindByIdAsync(usuarioId.ToString());
                if (usuarioIdentity == null)
                {
                    throw new Exception("No esta autorizado para eliminar este registro");
                }

                usucarioConsultaId = usuarioIdentity.Id.ToString();

                usuarioNegocio = await _usuarioRepository.ObtenerPorId(usuarioIdentity.Id);
                if (usuarioNegocio == null)
                {
                    throw new Exception("Para eliminar este registro, el ciudadano debe estar activo");
                }
            }

            var resultDto = await _expedienteRepository.ObtenerTodoslosExpedientesAsync(usuarioNegocio.Id);

            var result = resultDto.Adapt<List<GetAllExpedientesDto>>();

            foreach (var registro in result)
            {
                if (registro.TipoDocumentoId != Guid.Empty)
                {
                    var tipoDocDb = await _tipoDocumentoRepository.GetTipodocumntoByIdAsync(registro.TipoDocumentoId);
                    if (tipoDocDb != null)
                    {
                        registro.TipoDocumentoNombre = tipoDocDb.Nombre;
                        continue;
                    } else
                    {
                        registro.TipoDocumentoNombre = "Desconocido";
                    }
                }
            }

            return result;
        }
    }
}
