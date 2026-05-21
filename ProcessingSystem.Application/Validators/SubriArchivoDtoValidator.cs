using FluentValidation;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Validators
{
    public class SubriArchivoDtoValidator : AbstractValidator<SubirArchivoDto>
    {
        public SubriArchivoDtoValidator() 
        {
            RuleFor(x => x.ExpedienteId)
                .NotEmpty().WithMessage("El id del expediente es obligatorio");
            RuleFor(x => x.Archivo)
                .NotNull().WithMessage("El archivo es obligatorio")
                .Must(a => a != null && a.Length > 0).WithMessage("El archivo adjunto no puede estar vacio");
        }
    }
}
