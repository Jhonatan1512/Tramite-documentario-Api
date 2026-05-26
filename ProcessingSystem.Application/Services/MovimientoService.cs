using Mapster;
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
    public class MovimientoService : IMovimientoService
    {
        private readonly IMovimientoRepository _movimientoRepository;
        private readonly IExpedienteRepository _expedienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public MovimientoService(IMovimientoRepository movimientoRepository, IExpedienteRepository expedienteRepository,
            IUsuarioRepository usuarioRepository)
        {
            _movimientoRepository = movimientoRepository;
            _expedienteRepository = expedienteRepository;
            _usuarioRepository = usuarioRepository;
        }
        public async Task ActualizarMovimientoAsync(ActualizarMovimientoDto dto, Guid usuarioModificacionId, Guid idMovimiento)
        {
            var existeRegistro = await _movimientoRepository.ObtenerPorIdAsync(idMovimiento);
            if (existeRegistro == null)
            {
                throw new InvalidOperationException("No existe la referencia del registro");
            }       

            dto.Adapt(existeRegistro);
            existeRegistro.ReceptorId = usuarioModificacionId;
            existeRegistro.UsuarioModificacion = usuarioModificacionId.ToString();
            existeRegistro.Estado = EstadoMovimiento.Recibido;
            existeRegistro.FechaModificacion = DateTime.Now;

            await _movimientoRepository.ActualizarMovimientoAsync(existeRegistro);
        }

        public async Task RegistrarMovimiento(RegistrarMovimientoDto dto, Guid usuarioCreacionId)
        {
            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(dto.ExpedienteId);
            if (expedienteExiste == null) throw new KeyNotFoundException("El expediente no existe");

            var oficinaPersonalId = await _usuarioRepository.ObtenerOficinaPersonalAsync(usuarioCreacionId);

            var registraMovimiento = dto.Adapt<Movimiento>();
            registraMovimiento.UsuarioCreacion = usuarioCreacionId.ToString();
            registraMovimiento.EmisorId = usuarioCreacionId;
            registraMovimiento.OficinaOrigenId = Guid.Parse(oficinaPersonalId);
            registraMovimiento.Estado = EstadoMovimiento.Derivado;

            expedienteExiste.Estado = EstadoExpediente.EnProceso;
            expedienteExiste.UsuarioModificacion = usuarioCreacionId.ToString();
            expedienteExiste.FechaModificacion = DateTime.Now;

            await _movimientoRepository.CrearMovimientoAsync(registraMovimiento);
            await _expedienteRepository.ActualizarEstado(expedienteExiste);
        }
    }
}
