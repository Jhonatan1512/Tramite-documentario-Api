using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Interfaces;
using System.Security.Claims;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ICurrentUserService _currentUserService;

        public UsuariosController(IUsuarioRepository usuarioRepository, IUsuarioService usuarioService, ICurrentUserService currentUserService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _currentUserService = currentUserService;
        }

        [HttpPost("registrarse")]
        public async Task<ActionResult> Crear(UsuariosDto dto)
        {
            var reult = await _usuarioService.CrearUsuarioAsync(dto);
            return Ok(reult);
        }

        [HttpPut("actualizar-datos")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<IActionResult> ActualizarDatos([FromBody] ActualizarUsuarioDto dto)
        {
            await _usuarioService.ActualizarUsuarioAsync(UsuarioId, dto);
            return Ok(new { menssage = "Datos Actualizados" });
        }

        private Guid UsuarioId => _currentUserService.GetUserId();
    }
}
