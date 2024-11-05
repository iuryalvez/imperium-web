namespace backend.Dtos.DietaPersonalizada
{
    public class DietaPersonalizadaDto
    {
        public int Id { get; set; }
        public int RacaoId { get; set; }
        public int AcompanhamentoIndId { get; set; }
        public int TipoDieta { get; set; } // 1-manutenção, 2-crescimento, 3-lactação
        // public float QtdAlimento { get; set; } // calculado
        // public float EnergiaMetabolizavel { get; set; } // calculado
        public float PesoAtual { get; set; }
        public float PesoObjetivo { get; set; }
        public float PesoIdeal { get; set; }
    }
}