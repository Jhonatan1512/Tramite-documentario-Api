using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using System.Security.Claims;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public MovimientoController(IMovimientoService movimientoService, UserManager<IdentityUser<Guid>> userManager, ICurrentUserService currentUserService)
        {
            _movimientoService = movimientoService;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        [HttpPut("actualizar-movimiento/{id}")]
        [Authorize(Roles = "Personal")]
        public async Task<ActionResult> ActualizarMovimiento(ActualizarMovimientoDto dto, Guid id)
        {
            await _movimientoService.ActualizarMovimientoAsync(dto, UsuarioId, id);
            return Ok(new { mensaje = "Expediente recibido" });
        }

        [HttpPost("registra-movimiento")]
        [Authorize(Roles = "Personal")]
        public async Task<ActionResult> RegistrarMovimiento(RegistrarMovimientoDto dto)
        {
            await _movimientoService.RegistrarMovimiento(dto, UsuarioId);
            return Ok(new { mensaje = "Se registro el movimiento del expediente" });
        }

        private Guid UsuarioId => _currentUserService.GetUserId();
    }
}
