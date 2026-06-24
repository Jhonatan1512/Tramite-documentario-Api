using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class ActualizarContraseniaDtoValidator : AbstractValidator<ActualizarContrasenaDto>
    {
        public ActualizarContraseniaDtoValidator()
        {
            RuleFor(p => p.ContrasenaNueva)
                .ValidarPassword();
        }
    }
}
