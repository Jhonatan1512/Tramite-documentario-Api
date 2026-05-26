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
        private readonly ICurrentUserService _currentUser;

        public OficinasController(IOficinaService oficinaService, ICurrentUserService currentUser)
        {
            _oficinaService = oficinaService;
            _currentUser = currentUser;
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

        private Guid UsuarioId => _currentUser.GetUserId();

    }
}
