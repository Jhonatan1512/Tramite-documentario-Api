using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly RoleManager<Rol> _identityRole;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(UserManager<IdentityUser<Guid>> userManager, RoleManager<Rol> identityRole, IConfiguration configuration, IUsuarioRepository usuarioRepository)
        {
            _userManager = userManager;
            _identityRole = identityRole;
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
        }
        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                throw new Exception("El Email es incorrecto");
            }

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if(!passwordCorrect)
            {
                throw new Exception("La contraseña es incorrecta");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string nameToken = "User";
            string userId = user.Id.ToString();

            if (roles.Contains("Admin"))
            {
                nameToken = "Administrador del Sistema";
            } else
            {
                var ciudadano = await _usuarioRepository.ObtenerPorId(user.Id); 
                if (ciudadano != null)
                {
                    nameToken = $"{ciudadano.Nombre} {ciudadano.Apellidos}";
                    userId = ciudadano.Id.ToString();
                } else
                {
                    var personal = await _usuarioRepository.ObtenerPorId(user.Id);
                    if(personal != null)
                    {
                        nameToken = $"{personal.Nombre} {personal.Apellidos}";
                        userId = personal.Id.ToString();
                    }
                }
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", nameToken),
                new Claim("id", userId.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
