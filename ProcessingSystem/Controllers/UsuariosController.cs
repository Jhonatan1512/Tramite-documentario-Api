using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Interfaces;

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
            try
            {
                var reult = await _usuarioService.CrearUsuarioAsync(dto);
                return Ok(reult);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("actualizar-datos")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<IActionResult> ActualizarDatos([FromBody] ActualizarUsuarioDto dto)
        {
            try
            {
                var usuarioIdClaims = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(usuarioIdClaims))
                {
                    return Unauthorized();
                }

                Guid userId = Guid.Parse(usuarioIdClaims);
                await _usuarioService.ActualizarUsuarioAsync(userId, dto);
                return Ok(new {menssage = "Datos Actualizados"});

            } catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }
    }
}
