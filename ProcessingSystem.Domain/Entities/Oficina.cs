using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Entities
{
    public class Oficina : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public Guid? OficinaPadreId { get; set; } 
        public virtual Oficina? OficinaPadre { get; set; } 
        public virtual ICollection<Usuarios> Trabajadores { get; set; } = [];
    }
}
