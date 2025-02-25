using Microsoft.AspNetCore.Mvc;
using PlanoContasAPI.Models;

namespace PlanoContasAPI.Repositories.Interfaces
{
    public interface IPlanoContasRepository
    {
        Task<ActionResult<PlanoContas>> PostPlanoContas(PlanoContas planoDeContas);
        Task<ActionResult<PlanoContas>> DeletePlanoContas(PlanoContas planoDeContas);
        Task<ActionResult<IEnumerable<PlanoContas>>> GetPlanoContas();
    }
}
