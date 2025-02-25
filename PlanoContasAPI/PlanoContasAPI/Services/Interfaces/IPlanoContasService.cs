using Microsoft.AspNetCore.Mvc;
using PlanoContasAPI.Models;

namespace PlanoContasAPI.Services.Interfaces
{
    public interface IPlanoContasService 
    {
        Task<ActionResult<IEnumerable<PlanoContas>>> GetPlanoContas();
        Task<ActionResult<PlanoContas>> PostPlanoContas(PlanoContas planoDeContas);
        Task<IActionResult> DeletePlanoContas(int id);
        Task<ActionResult<string>> SuggestNextCode(string codigoPai, string tipoLancamento);
    }
}
