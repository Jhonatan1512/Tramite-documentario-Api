using Mapster;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;

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
            existeRegistro.FechaRecepcion = DateTime.Now;

            await _movimientoRepository.ActualizarMovimientoAsync(existeRegistro);
        }

        public async Task FinalizarMovimientoAsync(FinalizarMovimientoDto dto, Guid usuarioCreacionId)
        {
            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(dto.ExpedienteId);
            if (expedienteExiste == null) throw new KeyNotFoundException("El expediente no existe");

            if (dto.Estado != EstadoMovimiento.Atendido && dto.Estado != EstadoMovimiento.Rechazado)
            {
                throw new ArgumentException("Este endpoint solo permite los estados Atendido o Rechazado.");
            }

            var oficinaPersonalId = await _usuarioRepository.ObtenerOficinaPersonalAsync(usuarioCreacionId);

            var movimientoFinal = dto.Adapt<Movimiento>();
            movimientoFinal.UsuarioCreacion = usuarioCreacionId.ToString();
            movimientoFinal.EmisorId = usuarioCreacionId;
            movimientoFinal.OficinaOrigenId = Guid.Parse(oficinaPersonalId);
            movimientoFinal.OficinaDestinoId = Guid.Parse(oficinaPersonalId); 
            movimientoFinal.FechaCreacion = DateTime.Now;
            movimientoFinal.FechaRecepcion = DateTime.Now;

            switch (dto.Estado)
            {
                case EstadoMovimiento.Atendido:
                    movimientoFinal.Estado = EstadoMovimiento.Atendido;
                    expedienteExiste.Estado = EstadoExpediente.Finalizado;
                    break;

                case EstadoMovimiento.Rechazado:
                    movimientoFinal.Estado = EstadoMovimiento.Rechazado;
                    expedienteExiste.Estado = EstadoExpediente.Observado;
                    break;
            }

            expedienteExiste.UsuarioModificacion = usuarioCreacionId.ToString();
            expedienteExiste.FechaModificacion = DateTime.Now;

            await _movimientoRepository.CrearMovimientoAsync(movimientoFinal);
            await _expedienteRepository.ActualizarEstado(expedienteExiste);
        }

        public async Task RegistrarMovimiento(RegistrarMovimientoDto dto, Guid usuarioCreacionId)
        {
            if (dto.Estado == EstadoMovimiento.Atendido || dto.Estado == EstadoMovimiento.Rechazado)
            {
                throw new ArgumentException("Para finalizar o rechazar un expediente, utilice el proceso de finalización correspondiente.");
            }

            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(dto.ExpedienteId);
            if (expedienteExiste == null) throw new KeyNotFoundException("El expediente no existe");

            var oficinaPersonalId = await _usuarioRepository.ObtenerOficinaPersonalAsync(usuarioCreacionId);

            var registraMovimiento = dto.Adapt<Movimiento>();
            registraMovimiento.UsuarioCreacion = usuarioCreacionId.ToString();
            registraMovimiento.EmisorId = usuarioCreacionId;
            registraMovimiento.OficinaOrigenId = Guid.Parse(oficinaPersonalId);
            registraMovimiento.FechaCreacion = DateTime.Now;

            switch (dto.Estado)
            {
                case EstadoMovimiento.Derivado:
                    registraMovimiento.Estado = EstadoMovimiento.Derivado;
                    expedienteExiste.Estado = EstadoExpediente.EnProceso;
                    break;

                case EstadoMovimiento.Recibido:
                    registraMovimiento.Estado = EstadoMovimiento.Recibido;
                    break;
            }

            expedienteExiste.UsuarioModificacion = usuarioCreacionId.ToString();
            expedienteExiste.FechaModificacion = DateTime.Now;

            await _movimientoRepository.CrearMovimientoAsync(registraMovimiento);
            await _expedienteRepository.ActualizarEstado(expedienteExiste);
        }
    }
}