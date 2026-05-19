

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly ITipoDocumentoService _tipoDocumentoService;

        public TipoDocumentoController(ITipoDocumentoService tipoDocumentoService)
        {
            _tipoDocumentoService = tipoDocumentoService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearTipoDocumento(TipoDocumentoDto dto)
        {
            try
            {
                var usuario = User.FindFirst("id")?.Value;
                if(string.IsNullOrWhiteSpace(usuario))
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuario);
                var result = await _tipoDocumentoService.CrearTipoDocumentoAsync(userId, dto);
                return Ok(result);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("actualizar-tipo-documento/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarTipoDocumento(Guid id, ActualizarTipoDocumentoDto dto)
        {
            try
            {
                var usuarioIdClaims = User.FindFirst("id")?.Value;
                if (string.IsNullOrWhiteSpace(usuarioIdClaims))
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuarioIdClaims);
                await _tipoDocumentoService.ActuslizarTipoDocumentoAsync(id, userId, dto);
                return Ok(new { mensaje = "Registro Actualizado"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("lista-tipo-documentos")]
        public async Task<IActionResult> ObetenerTodos()
        {
            try
            {
                var result = await _tipoDocumentoService.ObtenerTodosAsync();
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
