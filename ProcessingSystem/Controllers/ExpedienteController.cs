using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpedienteController : ControllerBase
    {
        private readonly IExpedienteService _expedienteService;

        public ExpedienteController(IExpedienteService expedienteService)
        {
            _expedienteService = expedienteService;
        }

        [HttpPost("registrar")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> Crearexpediente(ExpedienteDto dto)
        {
            try
            {
                var usuaruiIdClaims = User.FindFirst("id")?.Value;
                if (string.IsNullOrWhiteSpace(usuaruiIdClaims))
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuaruiIdClaims);
                var result = await _expedienteService.CrearExpedienteAsync(userId, dto);
                return Ok(result);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("actualizar-expediente/{expedienteId}")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> ActualizarExpediente(Guid expedienteId, ExpedienteDto dto)
        {
            try
            {
                var usuarioIdClaims = User.FindFirst("id")?.Value;  
                if(usuarioIdClaims == null)
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuarioIdClaims);
                await _expedienteService.ActualizarExpedienteAsync(expedienteId, userId, dto);
                return Ok(new { mssage = "Datos del expediente actualizados" });

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{expedienteId}")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> Eliminarexpediente(Guid expedienteId)
        {
            try
            {
                var usuarioIdClaims = User.FindFirst("id")?.Value;
                if(usuarioIdClaims == null)
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuarioIdClaims);
                await _expedienteService.EliminarExpedienteService(expedienteId, userId);
                return Ok(new { message = "Registro eliminado" });

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("todos-los-expedientes")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var usuarioIdClaims = User.FindFirst("id")?.Value;
                if (usuarioIdClaims == null)
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuarioIdClaims);
                var result = await _expedienteService.GetExpedienteListAsync(userId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
