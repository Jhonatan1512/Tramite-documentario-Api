using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;

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
                var resultCrear = await _roleManager.CreateAsync(new Rol
                { 
                    Name = nombreRole,
                    NormalizedName = nombreRole.ToUpper()
                });

                if (!resultCrear.Succeeded)
                {
                    throw new Exception($"No se pudo crear el rol: {string.Join(", ", resultCrear.Errors.Select(e => e.Description))}");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, nombreRole))
            {
                var resultAsignar = await _userManager.AddToRoleAsync(user, nombreRole);
                if (!resultAsignar.Succeeded)
                {
                    throw new Exception($"No se pudo asignar el rol al usuario: {string.Join(", ", resultAsignar.Errors.Select(e => e.Description))}");
                }
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

            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                var errores = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errores);
            }

            return newUser.Id;
        }
    }
}
