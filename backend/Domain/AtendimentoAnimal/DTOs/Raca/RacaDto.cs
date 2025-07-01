using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    // DTO for returning breed data to the client
    // Comentário: DTO para retornar dados de raça para o cliente
    public class RacaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty; // Returning string for API friendliness
        public float? PesoMinimoReferencia { get; set; }
        public float? PesoMaximoReferencia { get; set; }
    }
}