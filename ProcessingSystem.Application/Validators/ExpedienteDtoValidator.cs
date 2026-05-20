using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class ExpedienteDtoValidator : AbstractValidator<ExpedienteDto>
    {
        public ExpedienteDtoValidator()
        {
            RuleFor(x => x.Asunto)
                .NotEmpty().WithMessage("El asunto no puede estar vacio")
                .NotNull().WithMessage("El asunto no puede ser nulo")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("El asunto no puede tener solo espacios en blanco");

            RuleFor(x => x.TipoDocumentoId)
                .NotEmpty().WithMessage("Debe seleccionar un tipo de documento válido");
        }
    }
}
