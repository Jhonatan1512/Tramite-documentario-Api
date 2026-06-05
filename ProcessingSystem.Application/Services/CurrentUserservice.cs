using Microsoft.AspNetCore.Http;
using ProcessingSystem.Application.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class CurrentUserservice : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserservice(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetOficinaId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                throw new UnauthorizedAccessException("No hay contexto de usuario");
            }

            var usuarioClaim = user.Claims
                .FirstOrDefault(c => c.Type.Equals("oficinaId", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrWhiteSpace(usuarioClaim) || !Guid.TryParse(usuarioClaim, out var parsedGuid))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado o token mal estructurado");
            }

            if (parsedGuid == Guid.Empty)
            {
                throw new UnauthorizedAccessException("El usuario con sesión activa no pertenece a ninguna oficina.");
            }

            return parsedGuid;
        }

        public Guid GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if(user == null)
            {
                throw new UnauthorizedAccessException("No hay contexto de usuario");
            }

            var usuarioClaim = user.FindFirst("id")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrWhiteSpace(usuarioClaim) || !Guid.TryParse(usuarioClaim, out var parsedGuid))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            return parsedGuid;
        }
    }
}
