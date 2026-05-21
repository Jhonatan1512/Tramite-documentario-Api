using Microsoft.EntityFrameworkCore;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using ProcessingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Infrastructure.Repositories
{
    public class DocumentoArchivoRepository : IDocumentoArchivoRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentoArchivoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarArchivoAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<DocumentoArchivo> CrearDocumentoArchivoAsync(DocumentoArchivo documentoArchivo)
        {
            await _context.DocumentoArchivos.AddAsync(documentoArchivo);
            await _context.SaveChangesAsync();
            return documentoArchivo;
        }

        public async Task<string?> EliminarArchivo(Guid archivoId)
        {
            var urlArchivo = await _context.DocumentoArchivos
                .Where(d => d.Id == archivoId)
                .Select(d => d.UrlArchivo)
                .FirstOrDefaultAsync();

            if (urlArchivo == null) return null!;

            await _context.DocumentoArchivos
                .Where(d => d.Id == archivoId)
                .ExecuteDeleteAsync();

            return urlArchivo;
        }

        public async Task<DocumentoArchivo?> ObtenerArchivoPorIdAsync(Guid id)
        {
            return await _context.DocumentoArchivos.FirstOrDefaultAsync(d =>  d.Id == id);
        }
    }
}
