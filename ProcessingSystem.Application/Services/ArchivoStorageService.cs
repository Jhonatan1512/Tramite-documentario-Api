using Microsoft.AspNetCore.Http;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Application.Services
{
    public class ArchivoStorageService : IArchivoStorageService
    {
        public Task EliminarArchivo(string urlArchivo)
        {
            if(string.IsNullOrWhiteSpace(urlArchivo)) return Task.CompletedTask;

            var limpiarUrl = urlArchivo.Trim('/');
            var pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", limpiarUrl);

            if(File.Exists(pathFile))
            {
                File.Delete(pathFile);
            }

            return Task.CompletedTask;
        }

        public async Task<string> GuardarArchivoAsync(IFormFile archivo)
        {
            var folderName = Path.Combine("wwwroot", "Uploads");
            var pathSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if(!Directory.Exists(pathSave))
            {
                Directory.CreateDirectory(pathSave);
            }

            var nombreOriginal = Path.GetFileNameWithoutExtension(archivo.FileName);
            var extension = Path.GetExtension(archivo.FileName);
            var nombreUnicoId = Guid.NewGuid().ToString();
            var nombreArchivoFisico = $"{nombreOriginal}_{nombreUnicoId}{extension}";
            var fullPath = Path.Combine(pathSave, nombreArchivoFisico);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/Uploads/{nombreArchivoFisico}";
        }

        public bool ExisteArchivo(string path, string rutaBaseWeb)
        {
            var rutaAbsoluta = CalcularRutaAbsoluta(path, rutaBaseWeb);
            return File.Exists(rutaAbsoluta);
        }

        public string ObtenerContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLower().Trim();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        public Stream ObtenerFlujoArchivo(string path, string rutaBaseWeb)
        {
            var rutaAbsoluta = CalcularRutaAbsoluta(path, rutaBaseWeb);
            return new FileStream(rutaAbsoluta, FileMode.Open, FileAccess.Read);
        }

        private string CalcularRutaAbsoluta(string path, string rutaBaseWeb)
        {
            var nombreFisico = Path.GetFileName(path);
            return Path.Combine(rutaBaseWeb, "Uploads", nombreFisico);
        }
    }
}
