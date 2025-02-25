using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanoContasAPI.Models;
using PlanoContasAPI.Repositories;
using PlanoContasAPI.Repositories.Interfaces;
using PlanoContasAPI.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PlanoContasAPI.Services
{
    public class PlanoContasService : ControllerBase, IPlanoContasService
    {
        private readonly PlanoContasContext _context;
        private readonly IPlanoContasRepository _planoContasRepository;

        public PlanoContasService(IPlanoContasRepository planoContasRepository,
                                  PlanoContasContext planoContasContext)
        {
            this._planoContasRepository = planoContasRepository;
            this._context = planoContasContext;
        }

        public async Task<ActionResult<IEnumerable<PlanoContas>>> GetPlanoContas()
        {
            var planoContas = await _planoContasRepository.GetPlanoContas();

            if (planoContas.Value.Count() == 0)
            {
                return NotFound("Não existem Plano de Contas cadastrados!");
            }

            return planoContas;
        }

        public async Task<ActionResult<PlanoContas>> PostPlanoContas(PlanoContas planoDeContas)
        {
            try
            {
                //código já existente
                var existeCodigoPlano = await _context.PlanoContas.Where(d => d.Ativo && d.Codigo == planoDeContas.Codigo).
                                                FirstOrDefaultAsync();
                if (existeCodigoPlano != null)
                {
                    return Content($"Código de Plano de Contas já existe no cadastro: {planoDeContas.Codigo}");
                }

                //carrega conta pai 
                string codigoPai = planoDeContas.Codigo.Length == 1 ? planoDeContas.Codigo : planoDeContas.Codigo.ToString().Substring(0, planoDeContas.Codigo.ToString().Length - 2);

                //verifica lançamento código pai
                var codigoPaiLanc = await _context.PlanoContas.
                                                Where(d => d.Ativo && d.AceitaLancamentos && d.Tipo == planoDeContas.Tipo && d.Codigo == codigoPai).
                                                OrderBy(o => o.Codigo).LastOrDefaultAsync();
                if (codigoPaiLanc != null)
                {
                    return Content($"Código de Plano de Contas aceita lançamentos e não pode ter contas filhas: {planoDeContas.Codigo}");
                }

                // verifica se é o mesmo tipo de conta
                string inicioCodigoFilho = planoDeContas.Codigo.Substring(0, 1);
                var codigoPlanoPai = await _context.PlanoContas.
                                            Where(d => d.Ativo && !d.AceitaLancamentos && d.Codigo.Substring(0, 1) == inicioCodigoFilho).
                                            FirstOrDefaultAsync();
                string inicioCodigoPai = codigoPlanoPai != null ? codigoPlanoPai.Codigo.Substring(0, 1) : "";
                if (codigoPlanoPai != null)
                {
                    if (inicioCodigoPai == inicioCodigoFilho && codigoPlanoPai.Tipo != planoDeContas.Tipo)
                    {
                        return Content($"Código de Plano de Contas devem obrigatoriamente ser do mesmo tipo do seu pai. " +
                                       $"Código Pai/Tipo: {codigoPlanoPai.Codigo}/{codigoPlanoPai.Tipo} - Filha/Tipo: {planoDeContas.Codigo}/{planoDeContas.Tipo}");
                    }
                }

                return await _planoContasRepository.PostPlanoContas(planoDeContas);
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserMessage", "Erro ao salvar Plano de Contas!");
                throw;
            }
        }

        public async Task<IActionResult> DeletePlanoContas(int id)
        {
            var planoDeContas = await _context.PlanoContas.Where(d => d.Ativo && d.Id == id).FirstOrDefaultAsync();
            if (planoDeContas == null)
            {
                return NotFound($"Não existe esse plano de contas para deleção: {id.ToString()}");
            }

            planoDeContas.Ativo = false;
            await _planoContasRepository.DeletePlanoContas(planoDeContas);

            return Content($"Registro inativado: {id.ToString()}");
        }

        public async Task<ActionResult<string>> SuggestNextCode(string codigoPai, string tipoLancamento)
        {
            var lastCode = await _context.PlanoContas
                                         .Where(w =>  w.Ativo && w.Tipo == tipoLancamento && w.Codigo.StartsWith(codigoPai.Substring(0, 1)))
                                         .OrderByDescending(p => p.Codigo)
                                         .FirstOrDefaultAsync();

            int indiceInicial = 0;
            var lastCodeInt = 0;
            var nextCode = "";

            if (lastCode == null)
            {
                indiceInicial = codigoPai.LastIndexOf(".") + 1;
                lastCodeInt = int.Parse(codigoPai.Substring(indiceInicial, codigoPai.Length - indiceInicial));
                nextCode = codigoPai;

            }
            else
            {
                indiceInicial = lastCode.Codigo.LastIndexOf(".") + 1;
                lastCodeInt = int.Parse(lastCode.Codigo.Substring(indiceInicial, lastCode.Codigo.Length - indiceInicial));
                nextCode = indiceInicial == 0 ? lastCode.Codigo.Substring(0, 1) + ".1" : lastCode.Codigo.Substring(0, indiceInicial) + ((lastCodeInt + 1).ToString());
            }

            if (Convert.ToInt32(lastCodeInt) >= 999)
            {
                var lastCodeFather = await _context.PlanoContas
                                                    .Where(w => w.Ativo && w.Tipo == tipoLancamento && !w.AceitaLancamentos)
                                                    .OrderByDescending(p => p.Codigo)
                                                    .FirstOrDefaultAsync();

                var existeCodigoPlano = await _context.PlanoContas
                                                      .Where(d => d.Ativo && d.Codigo == lastCodeFather.Codigo)
                                                      .FirstOrDefaultAsync();
                if (existeCodigoPlano == null)
                {
                    lastCodeInt = 1;
                    nextCode = lastCodeInt.ToString();
                }
                else
                {
                    lastCodeFather.Codigo = lastCodeFather.Codigo.Length == 1 ? lastCodeFather.Codigo.Substring(0, 1) + ".1" : lastCodeFather.Codigo;
                    indiceInicial = lastCodeFather.Codigo.LastIndexOf(".") + 1;
                    lastCodeInt = int.Parse(lastCodeFather.Codigo.Substring(indiceInicial, lastCodeFather.Codigo.Length - indiceInicial));
                    nextCode = lastCodeFather.Codigo.Substring(0, indiceInicial) + (lastCodeInt.ToString());

                    while (true)
                    {
                        existeCodigoPlano = await _context.PlanoContas
                                                          .Where(d => d.Ativo && d.Codigo == nextCode)
                                                          .FirstOrDefaultAsync();

                        if (existeCodigoPlano != null)
                        {
                            indiceInicial = nextCode.LastIndexOf(".") + 1;
                            lastCodeInt = int.Parse(nextCode.Substring(indiceInicial, nextCode.Length - indiceInicial));
                            nextCode = nextCode.Substring(0, indiceInicial) + ((lastCodeInt + 1).ToString());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            var existeOutroTipo = await _context.PlanoContas
                                                .Where(d => d.Ativo && d.Codigo == nextCode)
                                                .FirstOrDefaultAsync();
            if (existeOutroTipo != null)
            {
                if (existeOutroTipo.Tipo != tipoLancamento)
                {
                    return Content($"Existe essa conta: {codigoPai.ToString()} para o tipo de lançamento {existeOutroTipo.Tipo}");
                }
            }

            var tipoCode = nextCode.Length == 1 ? "Cadastrar conta Pai :" : "Cadastrar conta Filha: ";

            return Content($"{tipoCode}{nextCode}");
        }

    }
}
