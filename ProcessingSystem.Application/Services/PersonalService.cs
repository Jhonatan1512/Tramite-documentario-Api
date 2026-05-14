using Mapster;
using Microsoft.AspNetCore.Identity;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly IPersonalRepository _personalRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly IOficinaRepository _oficinaRepository;

        public PersonalService(IPersonalRepository personalRepository, UserManager<IdentityUser<Guid>> userManager, RoleManager<Rol> roleManager, IOficinaRepository oficinaRepository)
        {
            _personalRepository = personalRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _oficinaRepository = oficinaRepository;
        }
        public async Task<GetPersonalDto> CrearPersonalAsync(PersonalDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellidos) || string.IsNullOrWhiteSpace(dto.Dni) || 
                string.IsNullOrWhiteSpace(dto.UserId.ToString()) || string.IsNullOrWhiteSpace(dto.OficinaId.ToString()))
            {
                throw new Exception("Todos los campos son obligatorios");
            }

            if (dto.OficinaId.HasValue)
            {
                var oficinaExiste = await _oficinaRepository.GetByIdAsync(dto.OficinaId.Value);
                if(oficinaExiste == null)
                {
                    throw new Exception("La oficina especificada no existe");
                }
            }

            var dniExiste = await _personalRepository.ObtenerPorDni(dto.Dni);
            if (dniExiste != null)
            {
                throw new Exception("El DNI que esta intentando registrar ya existe en la BD");
            }

            var partesApellidos = dto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char inicial1 = partesApellidos.Length > 0 ? partesApellidos[0][0] : 'X';
            char inicial2 = partesApellidos.Length > 1 ? partesApellidos[1][0] : inicial1;

            string emailGenerado = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{dto.Dni}@ejemplo.gob.pe";
            var passwordgenerada = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{dto.Dni}*";

            var newUser = new IdentityUser<Guid>
            {
                UserName = emailGenerado,
                Email = emailGenerado,
                EmailConfirmed = true,
            };

            var identityResult = await _userManager.CreateAsync(newUser, passwordgenerada);
            if(!identityResult.Succeeded)
            {
                var errores = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new Exception(errores);
            }

            const string rolPersonal = "Personal";
            if (!await _roleManager.RoleExistsAsync(rolPersonal))
            {
                await _roleManager.CreateAsync(new Rol { Name = rolPersonal, NormalizedName = "PERSONAL" });
            }

            await _userManager.AddToRoleAsync(newUser, rolPersonal);

            var nuevoPersonal = dto.Adapt<Usuarios>();

            nuevoPersonal.UserId = newUser.Id;

            await _personalRepository.CrearPersonalAsync(nuevoPersonal);

            var result = nuevoPersonal.Adapt<GetPersonalDto>();
            result.Email = newUser.Email;

            return result;
        }
    }
}
