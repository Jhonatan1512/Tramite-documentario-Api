using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.DTOs
{
    public class OficinaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Siglas {  get; set; } = string.Empty;
        public Guid? OficinaPadreId { get; set; }
    }
}
