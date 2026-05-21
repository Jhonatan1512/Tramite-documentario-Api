using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class OficinaDtoValidator : AbstractValidator<OficinaDto>
    {
        public OficinaDtoValidator()
        {
            RuleFor(o => o.Nombre)
                .NotEmpty().WithMessage("El nombre de la oficina no puede estar vacio")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("El nombre no puede tener solo espacios en blanco");
        }
    }
}
