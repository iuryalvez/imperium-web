namespace backend.Domain.GestaoClinica.DTOs
{
    public class GetClinicaDashboardDto
    {
        public int Id { get; set; }
        public string? NomeClinica { get; set; }
        public string? NomeFantasia { get; set; }
        public string? Cnpj { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Whatsapp { get; set; }
        public string? Instagram { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool IsAtiva { get; set; }
        public string? LogoUrl { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Uf { get; set; }

        // Estatísticas para o Dashboard
        public int TotalUsuarios { get; set; }
        public int TotalAnimais { get; set; }
        public int TotalTutores { get; set; }
        public int TotalRacas { get; set; }
        public int TotalRacoes { get; set; }
        public int TotalConsultas { get; set; }
        public int TotalRelatorios { get; set; }
        public int TotalAcompanhamentos { get; set; }
    }
}