using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OficinasController : ControllerBase
    {
        private readonly IOficinaService _oficinaService;

        public OficinasController(IOficinaService oficinaService)
        {
            _oficinaService = oficinaService;
        }

        [HttpPost]
        public async Task<ActionResult> CrearOficina(OficinaDto dto)
        {
            try
            {
                var result = await _oficinaService.CrearOficinaAsync(dto);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
