using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Domain.Interfaces
{
    public interface IOficinaRepository
    {
        Task<Oficina> CrearOficinaAsync(Oficina oficina);
        Task<IEnumerable<Oficina>> GetAllAsync();
        Task<Oficina?> GetByIdAsync(Guid id);
    }
}
