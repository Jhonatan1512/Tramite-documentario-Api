using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenBlacklistService _blacklistService;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<IdentityUser<Guid>> userManager,
            IUsuarioRepository usuarioRepository, ITokenBlacklistService blacklistService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _usuarioRepository = usuarioRepository;
            _blacklistService = blacklistService;
            _tokenService = tokenService;
        }
        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                throw new Exception("Credenciales incorrectas");
            }

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if(!passwordCorrect)
            {
                throw new Exception("Credenciales incorrectas");
            }

            var estaActivo = await _usuarioRepository.ObtenerPorId(user.Id);
            if (estaActivo != null && estaActivo.EstaEliminado)
            {
                throw new Exception("El usuario esta inactivo.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string nameToken = "User";

            if (roles.Contains("Admin"))
            {
                nameToken = "Administrador del Sistema";
            } else
            {
                if(estaActivo == null)
                {
                    throw new Exception("Error");
                }
                nameToken = $"{estaActivo.Nombre} {estaActivo.Apellidos}";
            }

                return _tokenService.GenerarTokenAsync(user, roles, nameToken);            
        }

        public async Task LogOutAsync(string token)
        {
           await _blacklistService.InvalidTokenAsync(token);
        }
    }
}
