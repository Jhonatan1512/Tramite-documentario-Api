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
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly RoleManager<Rol> _roleManager;

        public UsuarioService(IUsuarioRepository usuarioRepository, UserManager<IdentityUser<Guid>> userManager, RoleManager<Rol> roleManager)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ActualizarUsuarioAsync(Guid id, ActualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
            {
                throw new Exception("El usuario no existe");
            }

            if(usuario.Dni != dto.Dni)
            {
                var dniExiste = await _usuarioRepository.ObtenerPorDniAsync(dto.Dni);
                if(dniExiste != null)
                {
                    throw new Exception("El dni ya esta registrado en la BD");
                }
            }

            usuario.Nombre = dto.Nombre;
            usuario.Apellidos = dto.Apellidos;
            usuario.Dni = dto.Dni;
            usuario.FechaModificacion = dto.FechaModificacion;
            usuario.UsuarioModificacion = usuario.Id.ToString();

            await _usuarioRepository.ActualizarUsuarioAsync(usuario);            
        }

        public async Task<GetUsuarioDto> CrearUsuarioAsync(UsuariosDto dto)
        {
            var existeDni = await _usuarioRepository.ObtenerPorDniAsync(dto.Dni);
            if (existeDni != null)
            {
                throw new Exception("El DNI que esta intentando registrar ya existe en la BD");                
            }

            var existeEmail = await _userManager.FindByEmailAsync(dto.Email);
            if(existeEmail != null)
            {
                throw new Exception("Ya existe un usuario con ese email");
            }

            var newUser = new IdentityUser<Guid>
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(newUser, dto.Password);
            if(!identityResult.Succeeded)
            {
                var errores = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new Exception(errores);
            }

            const string rolCiudadano = "Ciudadano";
            if(!await _roleManager.RoleExistsAsync(rolCiudadano))
            {
                await _roleManager.CreateAsync(new Rol { Name = rolCiudadano, NormalizedName = "CIUDADANO"});
            }
            await _userManager.AddToRoleAsync(newUser, rolCiudadano);

            var usuarioDto = dto.Adapt<Usuarios>();
            usuarioDto.UserId = newUser.Id;
            await _usuarioRepository.CrearUsuarioAsync(usuarioDto);

            var result = usuarioDto.Adapt<GetUsuarioDto>();
            result.Email = dto.Email;
            return result;
        }
    }
}
