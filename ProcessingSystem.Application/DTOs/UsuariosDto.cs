using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class UsuariosDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos {  get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
        ErrorMessage = "La contraseña debe tener mayúscula, minúscula, número y un carácter especial.")]
        public string Password {  get; set; } = string.Empty;
    }
     
    public class  GetUsuarioDto
    { 
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
