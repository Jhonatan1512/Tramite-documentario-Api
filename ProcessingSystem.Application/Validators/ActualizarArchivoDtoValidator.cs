using FluentValidation;
using ProcessingSystem.Application.DTOs;

namespace ProcessingSystem.Application.Validators
{
    public class ActualizarArchivoDtoValidator : AbstractValidator<ActualizarArchivoDto>
    {
        public ActualizarArchivoDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Debe especificar el id del archvio a modificar");

            RuleFor(x => x.ArchivoNuevo)
                .NotNull().WithMessage("Debe seleccionar un archivo")
                .Must(a => a != null && a.Length > 0).WithMessage("El archivo adjunto no puede estar vacio");
        }
    }
}
