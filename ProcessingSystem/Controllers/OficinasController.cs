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
    public class OficinasController : ControllerBase
    {
        private readonly IOficinaService _oficinaService;

        public OficinasController(IOficinaService oficinaService)
        {
            _oficinaService = oficinaService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearOficina(OficinaDto dto)
        {
            var result = await _oficinaService.CrearOficinaAsync(UsuarioId, dto);
            return Ok(result);
        }

        [HttpPut("actualizar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarOficina(Guid id, ActualizarOficinaDto dto)
        {
            await _oficinaService.ActualizarOficinaAsync(UsuarioId, id, dto);
            return Ok(new { mensaje = "Datos de la oficina actualizados"});
        }

        [HttpGet("listado-oficinas")]
        public async Task<ActionResult> ObtenerOficinas()
        {
            var result = await _oficinaService.GetOficinasAsync();
            return Ok(result);
        }

        private Guid UsuarioId 
        {
            get
            {
                var claimId = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(claimId) || !Guid.TryParse(claimId, out var parserGuid))
                {
                    throw new UnauthorizedAccessException("El usuario no esta autenticado o el token es invalido");
                }
                return parserGuid;
            }
        }
    }
}
