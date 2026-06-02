using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly IPersonalRepository _personalRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly IOficinaRepository _oficinaRepository;
        private readonly ICredencialesPersonalService _credencialesPersonal;

        public PersonalService(IPersonalRepository personalRepository, UserManager<IdentityUser<Guid>> userManager, 
            RoleManager<Rol> roleManager, IOficinaRepository oficinaRepository, ICredencialesPersonalService credencialesPersonal)
        {
            _personalRepository = personalRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _oficinaRepository = oficinaRepository;
            _credencialesPersonal = credencialesPersonal;
        }
        public async Task<GetPersonalDto> CrearPersonalAsync(Guid usuarioCreacionId, PersonalDto dto)
        {
            await ValidarOficinaDniAsync(dto.OficinaId, dto.Dni);
            
            var (emailGenerado, passwordGenerada) = _credencialesPersonal.GenerarParaPersonal(dto.Apellidos, dto.Dni);
            var newUser = new IdentityUser<Guid>
            {
                UserName = emailGenerado,
                Email = emailGenerado,
                EmailConfirmed = true,
            };

            var identityResult = await _userManager.CreateAsync(newUser, passwordGenerada);
            if(!identityResult.Succeeded)
            {
                var errores = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errores);
            }

            try
            {
                await SincronizarRolPerosnlaAsync(newUser);

                var nuevoPersonal = dto.Adapt<Usuarios>();
                nuevoPersonal.UserId = newUser.Id;
                nuevoPersonal.UsuarioCreacion = usuarioCreacionId.ToString();
                await _personalRepository.CrearPersonalAsync(nuevoPersonal);

                var result = nuevoPersonal.Adapt<GetPersonalDto>();
                
                result.Email = newUser.Email;
                return result;

            } catch (Exception)
            {
                await _userManager.DeleteAsync(newUser);
                throw;
            }
        }

        public async Task ActualizarPersonalAsync(Guid personalId, Guid usuarioId, ActualizarPersonalDto dto)
        {
            var personalExiste = await _personalRepository.GetByIdAsync(personalId);
            if(personalExiste == null)
            {
                throw new KeyNotFoundException("El ID del usuario no existe");
            }

            if (personalExiste.Dni != dto.Dni)
            {
                await ValidarOficinaDniAsync(dto.OficinaId, dto.Dni);
            } else if (dto.OficinaId.HasValue)
            {
                await ValidarOficinaDniAsync(dto.OficinaId, null!);
            }

            if(personalExiste.Apellidos != dto.Apellidos || personalExiste.Dni != dto.Dni)
            {
                await _credencialesPersonal.ActualizarCredenciales(personalExiste.UserId!.Value, dto.Apellidos, dto.Dni);
            }

            dto.Adapt(personalExiste);
            personalExiste.FechaModificacion = DateTime.Now;
            personalExiste.UsuarioModificacion = usuarioId.ToString();

            await _personalRepository.ActualizarDatosPersonalAsync(personalExiste);
        }

        public async Task<IEnumerable<GetPersonalDto>> ObtenerTodosAsync()
        {
            var personalDto = await _personalRepository.GetAll();
            if (!personalDto.Any()) return Enumerable.Empty<GetPersonalDto>();

            var usuariosRolPersonal = await _userManager.GetUsersInRoleAsync("Personal");
            var idsPersonal = usuariosRolPersonal.Select(u => u.Id).ToHashSet(); 

            var personalFiltrado = personalDto
                .Where(p => p.UserId.HasValue && idsPersonal.Contains(p.UserId.Value))
                .ToList();

            if (!personalFiltrado.Any()) return Enumerable.Empty<GetPersonalDto>();

            var result = personalFiltrado.Adapt<List<GetPersonalDto>>();

            var userIds = personalFiltrado.Select(p => p.UserId!.Value).Distinct().ToList();
            var listaEmails = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            var emailDictionary = listaEmails.ToDictionary(u => u.Id, u => u.Email);

            var oficinasDb = await _oficinaRepository.GetAllAsync();
            var oficinaDictionary = oficinasDb.ToDictionary(o => o.Id, o => o.Nombre);

            var personalMap = personalFiltrado.ToDictionary(p => p.Dni);

            foreach (var user in result)
            {
                if (personalMap.TryGetValue(user.Dni, out var entidad) && entidad.UserId.HasValue)
                {
                    if (emailDictionary.TryGetValue(entidad.UserId.Value, out var email))
                    {
                        user.Email = email!;
                    }
                }

                if (user.OficinaId != Guid.Empty && oficinaDictionary.TryGetValue(user.OficinaId, out var nombreOficina))
                {
                    user.NombreOficina = nombreOficina;
                }
                else
                {
                    user.NombreOficina = "Sin oficina";
                }
            }
            return result; 
        }

        public async Task<string> EliminarUsuario(Guid personalId, Guid usuarioId, DesactivarActivarPersonalDto dto)
        {
            var personalExiste = await _personalRepository.GetByIdAsync(personalId);
            if(personalExiste == null)
            {
                throw new KeyNotFoundException("El id del usuario no existe");
            }

            dto.Adapt(personalExiste);
            personalExiste!.FechaModificacion = DateTime.Now;
            personalExiste!.UsuarioModificacion = usuarioId.ToString();

            await _personalRepository.ActualizarDatosPersonalAsync(personalExiste);

            return personalExiste.EstaEliminado
                ? "El usuario ha sido descativado"
                : "El usuario ha sido activado";
        }

        private async Task ValidarOficinaDniAsync(Guid? oficinaId, string dni)
        {
            if (oficinaId.HasValue && await _oficinaRepository.GetByIdAsync(oficinaId.Value) == null)
            {
                throw new KeyNotFoundException("La oficina especificada no existe en el sistema");
            }

            if (await _personalRepository.ObtenerPorDni(dni) != null)
            {
                throw new InvalidOperationException("El DNI que esta intentando registra ya existe en la BD");
            }
        }

        private async Task SincronizarRolPerosnlaAsync(IdentityUser<Guid> user)
        {
            const string rolPersonal = "Personal";
            if (!await _roleManager.RoleExistsAsync(rolPersonal))
            {
                await _roleManager.CreateAsync(new Rol { Name = rolPersonal, NormalizedName = "PERSONAL" });
            }
            await _userManager.AddToRoleAsync(user, rolPersonal);
        }
    }
}
