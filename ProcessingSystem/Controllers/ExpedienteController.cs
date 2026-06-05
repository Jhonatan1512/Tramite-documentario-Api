using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using System.Security.Claims;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpedienteController : ControllerBase
    {
        private readonly IExpedienteService _expedienteService;
        private readonly ICurrentUserService _currentUser;

        public ExpedienteController(IExpedienteService expedienteService, ICurrentUserService currentUser)
        {
            _expedienteService = expedienteService;
            _currentUser = currentUser;
        }

        [HttpPost("registrar")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> Crearexpediente(ExpedienteDto dto)
        {
            var result = await _expedienteService.CrearExpedienteAsync(UsuarioId, dto);
            return Ok(result);
        }

        [HttpPut("actualizar-expediente/{expedienteId}")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> ActualizarExpediente(Guid expedienteId, ExpedienteDto dto)
        {
            await _expedienteService.ActualizarExpedienteAsync(expedienteId, UsuarioId, dto);
            return Ok(new { mssage = "Datos del expediente actualizados" });
        }

        [HttpDelete("{expedienteId}")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> Eliminarexpediente(Guid expedienteId)
        {
            await _expedienteService.EliminarExpedienteService(expedienteId, UsuarioId);
            return Ok(new {message = "Registro eliminado"});
        }

        [HttpGet("todos-los-expedientes")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> GetAll()
        {
            var result = await _expedienteService.GetExpedienteListAsync(UsuarioId);
            return Ok(result);
        }

        [HttpGet("lista-epedientes-perfil")]
        public async Task<ActionResult> ObtenerExpedientesPorPerfil()
        {
            var result = await _expedienteService.ObtenerExpidientesPorPerfil();
            return Ok(result);
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
