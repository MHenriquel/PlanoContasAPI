using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PlanoContasAPI.Models;
using PlanoContasAPI.Repositories.Interfaces;

namespace PlanoContasAPI.Repositories
{
    public class PlanoContasRepository : ControllerBase, IPlanoContasRepository
    {
        private readonly PlanoContasContext _contextRepository;
        public PlanoContasRepository(PlanoContasContext planoContasContext)
        {
            this._contextRepository = planoContasContext;
        }

        public async Task<ActionResult<PlanoContas>> PostPlanoContas(PlanoContas planoDeContas)
        {
            try
            {
                _contextRepository.PlanoContas.Add(planoDeContas);
                await _contextRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserMessage", "Erro ao salvar Planos de Contas!");
                throw;
            }

            return planoDeContas;
        }

        public async Task<ActionResult<PlanoContas>> DeletePlanoContas(PlanoContas planoDeContas)
        {
            try
            {
                _contextRepository.PlanoContas.Update(planoDeContas);
                await _contextRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserMessage", "Erro ao inativar Planos de Contas!");
                throw;
            }

            return planoDeContas;
        }

        public async Task<ActionResult<IEnumerable<PlanoContas>>> GetPlanoContas()
        {
            try
            {
                return await _contextRepository.PlanoContas
                                               .Where(p => p.Ativo)
                                               .OrderBy(o => o.Codigo)
                                               .ToListAsync();
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserMessage", "Erro ao buscar os Planos de Contas!");
                throw;
            }
        }
    }
}
