using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class CiudadanoActualizacionDtoValidator : AbstractValidator<ActualizarUsuarioDto>
    {
        public CiudadanoActualizacionDtoValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(c => c.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorio");

            RuleFor(c => c.Dni)
                .Matches(@"^\d{8}$").WithMessage("El DNI debe tener exactamente 8 dígitos numéricos")
                .NotEmpty().WithMessage("El DNI es obligatorio");
        }
    }
}
