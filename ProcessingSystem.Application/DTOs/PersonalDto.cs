using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class PersonalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos {  get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid? OficinaId { get; set; }
    }

    public class GetPersonalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
