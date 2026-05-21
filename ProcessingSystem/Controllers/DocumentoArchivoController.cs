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

        public DocumentoArchivoController(IDocumentoArchivoService documentoArchivoService)
        {
            _documentoArchivoService = documentoArchivoService;
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
