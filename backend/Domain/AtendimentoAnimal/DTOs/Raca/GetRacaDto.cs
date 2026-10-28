using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class GetRacaDto
    {
        public int Id { get; set; }
        public int? ClinicaId { get; set; }
        public string? NomeClinica { get; set; } // preenchido na listagem de cópias do admin
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public Porte? Porte { get; set; } // enviado como número, igual ao formulário
        public CategoriaMetabolica CategoriaMetabolica { get; set; } // enviado como número, igual ao formulário
        public double? PesoMinimoReferenciaFemea { get; set; }
        public double? PesoMaximoReferenciaFemea { get; set; }
        public double? PesoMinimoReferenciaMacho { get; set; }
        public double? PesoMaximoReferenciaMacho { get; set; }
        public string? Referencia { get; set; }
        public DateTime DataCadastro { get; set; }

        /// <summary>Raça do sistema (compartilhada entre as clínicas).</summary>
        public bool IsSistema { get; set; }
        public bool IsAtivo { get; set; }

        /// <summary>Raça do sistema oculta pela clínica que está consultando.</summary>
        public bool IsOculta { get; set; }
    }
}
