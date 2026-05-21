using Mapster;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class DocumentoArchivoService : IDocumentoArchivoService
    {
        private readonly IDocumentoArchivoRepository _documentoArchivoRepository;
        private readonly IExpedienteRepository _expedienteRepository;
        private readonly IArchivoStorageService _storageService;

        public DocumentoArchivoService(IDocumentoArchivoRepository documentoArchivoRepository, 
            IExpedienteRepository expedienteRepository, IArchivoStorageService storageService)
        {
            _documentoArchivoRepository = documentoArchivoRepository;
            _expedienteRepository = expedienteRepository;
            _storageService = storageService;
        }

        public async Task ActualizarArchivoAsync(Guid usuarioId, ActualizarArchivoDto dto)
        {
            var archivoExiste = await _documentoArchivoRepository.ObtenerArchivoPorIdAsync(dto.Id);
            if(archivoExiste == null)
            {
                throw new KeyNotFoundException("El archivo que esta intentando actualizar no existe");
            }

            var urlArchivoViejo = archivoExiste.UrlArchivo;

            string nuevaUrl = await _storageService.GuardarArchivoAsync(dto.ArchivoNuevo);

            archivoExiste.UrlArchivo = nuevaUrl;
            archivoExiste.NombreArchivo = Path.GetFileNameWithoutExtension(dto.ArchivoNuevo.FileName);
            archivoExiste.Extension = Path.GetExtension(dto.ArchivoNuevo.FileName).ToLower();            
            archivoExiste.Tamanio = dto.ArchivoNuevo.Length;
            archivoExiste.UsuarioModificacion = usuarioId.ToString();
            archivoExiste.FechaModificacion = DateTime.Now;

            await _documentoArchivoRepository.ActualizarArchivoAsync();

            await _storageService.EliminarArchivo(urlArchivoViejo);
        }

        public async Task EliminarArchivoAsync(Guid archivoId, Guid usuarioId)
        {
            var archivExiste = await _documentoArchivoRepository.EliminarArchivo(archivoId);
            if(archivExiste == null )
            {
                throw new Exception("El archivo que esta intentando elimnar no existe");
            }                     

            await _storageService.EliminarArchivo(archivExiste);
        }

        public async Task<DocumentoArchivoDto> SubirArchivoAsync(Guid usuarioId, SubirArchivoDto dto)
        {
            var expedienteExiste = await _expedienteRepository.BuscarExpedientePorIdAsync(dto.ExpedienteId);
            if(expedienteExiste == null)
            {
                throw new Exception("El expediente no existe");
            }

            var urlArchivo = await _storageService.GuardarArchivoAsync(dto.Archivo);

            var nuevoArchivo = dto.Adapt<DocumentoArchivo>();
            nuevoArchivo.UrlArchivo = urlArchivo;
            nuevoArchivo.NombreArchivo = Path.GetFileNameWithoutExtension(dto.Archivo.FileName);
            nuevoArchivo.Extension = Path.GetExtension(dto.Archivo.FileName).ToLower();
            nuevoArchivo.UsuarioCreacion = usuarioId.ToString();
            nuevoArchivo.FechaCreacion = DateTime.Now;
            nuevoArchivo.Tamanio = dto.Archivo.Length;

            var result = await _documentoArchivoRepository.CrearDocumentoArchivoAsync(nuevoArchivo);
            return result.Adapt<DocumentoArchivoDto>();
        }
    }
}
