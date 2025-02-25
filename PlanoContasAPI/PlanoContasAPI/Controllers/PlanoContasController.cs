using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanoContasAPI.Models;
using PlanoContasAPI.Services;
using PlanoContasAPI.Services.Interfaces;

namespace PlanoContasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoContasController : ControllerBase
    {
        private readonly PlanoContasContext _context;
        private readonly IPlanoContasService _planoContasService;

        public PlanoContasController(IPlanoContasService planoContasService, PlanoContasContext context)
        {
            _planoContasService = planoContasService;
            _context = context;
        }

        // GET: api/PlanoContas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanoContas>>> GetPlanoContas()
        {
            return await _planoContasService.GetPlanoContas();
        }

        // POST: api/PlanoContas
        [HttpPost]
        public async Task<ActionResult<PlanoContas>> PostPlanoContas(PlanoContas planoDeContas)
        {
            return await _planoContasService.PostPlanoContas(planoDeContas);
        }

        // DELETE: api/PlanoContas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanoContas(int id)
        {
            return await _planoContasService.DeletePlanoContas(id);
        }

        // GET: api/PlanoContas/suggest-code
        [HttpGet("suggest-code")]
        public async Task<ActionResult<string>> SuggestNextCode(string codigoPai, string tipoLancamento)
        {
            return await _planoContasService.SuggestNextCode(codigoPai, tipoLancamento);
        }
    }
}
