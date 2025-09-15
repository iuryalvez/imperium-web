namespace backend.Domain.AtendimentoAnimal
{
    public class AnimalCompletoDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string Especie { get; set; }
        public string Sexo { get; set; }
        public int Idade { get; set; }
        public string FaseVida { get; set; }
        public double? PesoAtual { get; set; }
        public string? CondicaoCorporal { get; set; }

        public RacaResumidaDto Raca { get; set; }
        public List<TutorResumidoDto> Tutores { get; set; }
        public AcompanhamentoDto Acompanhamento { get; set; }
    }

    public class RacaResumidaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class TutorResumidoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
    }

    public class AcompanhamentoDto
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public List<RelatorioDetalhadoDto> Relatorios { get; set; } // A linha do tempo
    }

    public class RelatorioDetalhadoDto
    {
        public int Id { get; set; }
        public DateTime DataConsulta { get; set; }
        public string ProfissionalResponsavel { get; set; }

        // Dados do relatório
        public double Peso { get; set; }
        public string CondicaoCorporal { get; set; }
        public string GrauAtividade { get; set; }

        public DietaResumidaDto? Dieta { get; set; }
    }

    public class DietaResumidaDto
    {
        public int Id { get; set; }
        public string TipoDieta { get; set; }
        public double QuantidadeAlimentoGramas { get; set; }
        public double NecessidadeEnergeticaKcal { get; set; }
        public string RacaoUtilizada { get; set; }
        public string? Observacoes { get; set; }
        public DateOnly DataCriacao { get; set; }
    }
}