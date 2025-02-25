using System.ComponentModel.DataAnnotations;

namespace PlanoContasAPI.Models
{
    public class PlanoContas
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string NomedaConta { get; set; }
        public required string Tipo { get; set; }
        public required bool AceitaLancamentos { get; set; }
        public required bool Ativo { get; set; }
    }
}
