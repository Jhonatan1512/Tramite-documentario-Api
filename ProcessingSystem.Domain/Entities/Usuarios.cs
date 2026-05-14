using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Entities
{
    public class Usuarios : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public Guid? OficinaId {  get; set; } 
        public virtual Oficina? Oficina { get; set; }
    }

    public class Rol : IdentityRole<Guid> { } 

}
