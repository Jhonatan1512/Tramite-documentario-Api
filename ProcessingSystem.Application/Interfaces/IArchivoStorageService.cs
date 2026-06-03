using Microsoft.AspNetCore.Http;
using ProcessingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface IArchivoStorageService
    {
        Task<string> GuardarArchivoAsync(IFormFile archivo);
        Task EliminarArchivo(string urlArchivo);
        bool ExisteArchivo(string path, string rutaBaseWeb);
        Stream ObtenerFlujoArchivo(string path, string rutaBaseWeb);
        string ObtenerContentType(string path);
        
    }
}
