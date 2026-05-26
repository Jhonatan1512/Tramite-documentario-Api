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

        public UsuariosController(IUsuarioRepository usuarioRepository, IUsuarioService usuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize(Roles = "Personal")]
        public async Task<ActionResult> Gett()
        {
            try
            {
                var result = await _usuarioRepository.GetAll();
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        private Guid UsuarioId
        {
            get
            {
                var usuarioClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(usuarioClaim) || !Guid.TryParse(usuarioClaim, out var parserGuid))
                {
                    throw new UnauthorizedAccessException("El usuario no esta autenticado o el token es invalido");
                }
                return parserGuid;
            }
        }
    }
}
