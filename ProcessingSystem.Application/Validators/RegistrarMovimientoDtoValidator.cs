using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class RegistrarMovimientoDtoValidator : AbstractValidator<RegistrarMovimientoDto>
    {
        public RegistrarMovimientoDtoValidator()
        {
            RuleFor(m => m.ComentarioDerivacion)
                .NotEmpty().WithMessage("El comentario no puede estar vacio")
                .Must(m => !string.IsNullOrWhiteSpace(m)).WithMessage("El comentario no puede tener solo espacios vacios");
        }
    }
}
