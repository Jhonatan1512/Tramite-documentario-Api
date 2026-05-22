using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class CredencialesPersonalService : ICredencialesPersonalService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public CredencialesPersonalService(UserManager<IdentityUser<Guid>> userManager)
        {
            _userManager = userManager;
        }
        public async Task ActualizarCredenciales(Guid identityUser, string apellidos, string dni)
        {
            var usuarioExiste = await _userManager.FindByIdAsync(identityUser.ToString());
            if (usuarioExiste == null)
            {
                throw new KeyNotFoundException("No se encontro la cuenta de seguridad de identity");
            }

            var (emailGenerado, passwordGenerada) = GenerarParaPersonal(apellidos, dni);

            usuarioExiste.UserName = emailGenerado;
            usuarioExiste.Email = emailGenerado;

            var updateResult = await _userManager.UpdateAsync(usuarioExiste);
            if (!updateResult.Succeeded)
            {
                var erroresUpdate = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Errores al actualizar identity {erroresUpdate}");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuarioExiste);
            var resultPassord = await _userManager.ResetPasswordAsync(usuarioExiste, token, passwordGenerada);

            if (!resultPassord.Succeeded)
            {
                throw new InvalidOperationException(resultPassord.Errors.ToString());
            }
        }

        public (string Email, string Password) GenerarParaPersonal(string apellidos, string dni)
        {
            var partesApellidos = apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char i1 = partesApellidos.Length > 0 ? char.ToLower(partesApellidos[0][0]) : 'x';
            char i2 = partesApellidos.Length > 1 ? char.ToLower(partesApellidos[1][0]) : i1;

            string email = $"{i1}{i2}{dni}@ejemplo.gob.pe";
            string password = $"{char.ToUpper(i1)}{i2}{dni}*";

            return (email, password);
        }
    }
}
