using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class PersonalDtoValidator : AbstractValidator<PersonalDto>
    {
        public PersonalDtoValidator()
        {
            RuleFor(p => p.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío")
                .Must(n => n != null && n.Length > 0).WithMessage("El nombre no puede tener solo espacions en blanco");

            RuleFor(p => p.Apellidos)
                .NotEmpty().WithMessage("El nombre no puede estar vacío")
                .Must(a => a != null && a.Length > 0).WithMessage("Los apellidos no puede tener solo espacions en blanco");

            RuleFor(p => p.Dni)
                .NotEmpty().WithMessage("El DNI no puede estar vacío")
                .Matches(@"^\d{8}$").WithMessage("El DNI debe tener exactamente 8 dígitos numéricos");
        }

    }
}
