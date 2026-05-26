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
    [Authorize]
    public class PersonalController : ControllerBase
    {
        private readonly IPersonalService _personalService;
        private readonly ICurrentUserService _currentUser;

        public PersonalController(IPersonalService personalService, ICurrentUserService currentUser)
        {
            _personalService = personalService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CearPersonal(PersonalDto dto)
        {
            var result = await _personalService.CrearPersonalAsync(UsuarioId, dto);
            return Ok(result);
        }

        [HttpPut("actualizar-datos/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarDatos(Guid id, ActualizarPersonalDto dto)
        {
            await _personalService.ActualizarPersonalAsync(id, UsuarioId, dto);
            return Ok("Datos del usuario actualizados");
        }

        [HttpGet("listado-personal")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> ObetenerTodos()
        {
            var result = await _personalService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpPut("eliminar-usuario/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EliminarUsuario(Guid id, DesactivarActivarPersonalDto dto)
        {
            string mensaje = await _personalService.EliminarUsuario(id, UsuarioId, dto);
            return Ok(new {message = mensaje});
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
