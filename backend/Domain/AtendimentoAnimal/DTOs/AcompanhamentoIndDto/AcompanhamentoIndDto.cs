using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;

namespace backend.Domain.AtendimentoAnimal.DTOs.Acompanhamento
{
    public class GetAcompanhamentoDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string NomeAnimal { get; set; } = string.Empty;
        public DateOnly DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }

        // A lista de todos os relatórios (com suas dietas) feitos para este animal
        public List<GetRelatorioDto> Relatorios { get; set; } = new();
    }
}