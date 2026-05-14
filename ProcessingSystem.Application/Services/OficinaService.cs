using Mapster;
using MapsterMapper;
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
    public class OficinaService : IOficinaService
    {
        private readonly IOficinaRepository _oficinaRepository;
        private readonly IMapper _mapper;

        public OficinaService(IOficinaRepository oficinaRepository, IMapper mapper)
        {
            _oficinaRepository = oficinaRepository;
            _mapper = mapper;
        }
        public async Task<OficinaDto> CrearOficinaAsync(OficinaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Siglas))
            {
                throw new Exception("Todos los campos son obligatorios");
            }

            if (dto.OficinaPadreId.HasValue)
            {
                var padreExiste = await _oficinaRepository.GetByIdAsync(dto.OficinaPadreId.Value);
                if (padreExiste == null)
                {
                    throw new Exception("La oficina superior especificada no existe");
                }
            }
            var nuevaOficina = dto.Adapt<Oficina>();

            var result = await _oficinaRepository.CrearOficinaAsync(nuevaOficina);

            return result.Adapt<OficinaDto>();
        }
    }
}
