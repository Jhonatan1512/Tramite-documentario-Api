using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class ActualizarMovimientoDtoValidator : AbstractValidator<ActualizarMovimientoDto>
    {
        public ActualizarMovimientoDtoValidator()
        {
            RuleFor(m => m.ComentarioFinal)
                .NotEmpty().WithMessage("El comentario no puede estar vacio")
                .Must(m => !string.IsNullOrWhiteSpace(m)).WithMessage("El comentario no puede tener solo espacios en blanco");
        }
    }
}
