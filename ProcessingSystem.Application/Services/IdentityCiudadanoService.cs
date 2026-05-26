using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class IdentityCiudadanoService : IIdentityCiudadanoiService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly RoleManager<Rol> _roleManager;

        public IdentityCiudadanoService(UserManager<IdentityUser<Guid>> userManager, RoleManager<Rol> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task AsignarRoleAsync(Guid identityRole, string nombreRole)
        {
            var user = await _userManager.FindByIdAsync(identityRole.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario de seguridad no encontrado");
            }

            if(!await _roleManager.RoleExistsAsync(nombreRole))
            {
                await _roleManager.CreateAsync(new Rol
                {
                    Name = nombreRole,
                    NormalizedName = nombreRole.ToUpper()
                });

                await _userManager.AddToRoleAsync(user, nombreRole);
            }
        }

        public async Task<Guid> CrearCuentaAsync(string email, string password)
        {
            var newUser = new IdentityUser<Guid>
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                var errores = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errores);
            }

            return newUser.Id;
        }
    }
}
