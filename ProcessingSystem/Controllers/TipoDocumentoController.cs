

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
        private readonly ICurrentUserService _currentUser;

        public TipoDocumentoController(ITipoDocumentoService tipoDocumentoService, ICurrentUserService currentUser)
        {
            _tipoDocumentoService = tipoDocumentoService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearTipoDocumento(TipoDocumentoDto dto)
        {
            var result = await _tipoDocumentoService.CrearTipoDocumentoAsync(UsuarioId, dto);
            return Ok(result);
        }

        [HttpPut("actualizar-tipo-documento/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarTipoDocumento(Guid id, ActualizarTipoDocumentoDto dto)
        {
            await _tipoDocumentoService.ActuslizarTipoDocumentoAsync(id, UsuarioId, dto);
            return Ok(new { mensaje = "Registro Actualizado" });
        }

        [HttpGet("lista-tipo-documentos")]
        public async Task<IActionResult> ObetenerTodos()
        {
            var result = await _tipoDocumentoService.ObtenerTodosAsync();
            return Ok(result);
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
