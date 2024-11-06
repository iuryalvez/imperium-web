namespace backend.Dtos.Animal
{
    public class GetAnimaisDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Especie { get; set; } // 1-cao, 2-gato
        public int Sexo { get; set; } // 1-macho, 2-fêmea
    }
}