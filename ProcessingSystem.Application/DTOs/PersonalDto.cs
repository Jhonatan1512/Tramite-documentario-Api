
namespace ProcessingSystem.Application.DTOs
{
    public class PersonalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos {  get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public Guid? OficinaId { get; set; }
    }

    public class ActualizarPersonalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public Guid? OficinaId { get; set; }
    }

    public class GetPersonalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid OficinaId { get; set; } 
        public string NombreOficina {  get; set; } = string.Empty; 
    }

    public class DesactivarActivarPersonalDto
    {
        public bool EstaEliminado { get; set; }
    }
}
