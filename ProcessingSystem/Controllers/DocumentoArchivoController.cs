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
        private readonly IWebHostEnvironment _env;

        public DocumentoArchivoController(IDocumentoArchivoService documentoArchivoService, ICurrentUserService currentUser, IWebHostEnvironment env)
        {
            _documentoArchivoService = documentoArchivoService;
            _currentUser = currentUser;
            _env = env;
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

        [HttpGet("ver/{id}")]
        public async Task<ActionResult> ObtenerArchivo(Guid id)
        {
            var result = await _documentoArchivoService.ObtenerArchivAsync(id, _env.WebRootPath, UsuarioId);
            return File(result!.Contenido, result.ContentType);
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
