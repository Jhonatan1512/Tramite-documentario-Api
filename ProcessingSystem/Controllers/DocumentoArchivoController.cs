using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using System.Security.Claims;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentoArchivoController : ControllerBase
    {
        private readonly IDocumentoArchivoService _documentoArchivoService;
        private readonly ICurrentUserService _currentUser;

        public DocumentoArchivoController(IDocumentoArchivoService documentoArchivoService, ICurrentUserService currentUser)
        {
            _documentoArchivoService = documentoArchivoService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> SubirArchivo([FromForm] SubirArchivoDto dto)
        {
            var result = await _documentoArchivoService.SubirArchivoAsync(UsuarioId, dto);
            return Ok(result);
        }

        [HttpDelete("{archivoId}")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> EliminarArchivo(Guid archivoId)
        {
            await _documentoArchivoService.EliminarArchivoAsync(archivoId, UsuarioId);
            return Ok(new {mensaje = "Registro de archivo eliminado"});
        }

        [HttpPut("actualizar-archivo")]
        [Authorize(Roles = "Ciudadano")]
        public async Task<ActionResult> ActualizarArchivo(ActualizarArchivoDto dto)
        {
            await _documentoArchivoService.ActualizarArchivoAsync(UsuarioId, dto);
            return Ok(new { mensaje = "Archivo actualizado"});
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
