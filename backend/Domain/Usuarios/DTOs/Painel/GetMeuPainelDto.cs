namespace backend.Domain.Usuarios.DTOs.Painel
{
    /// <summary>
    /// Resumo do usuário logado na clínica (GET /api/clinica/meu-painel).
    /// As seções de cada perfil só vêm preenchidas para quem tem o perfil; um usuário com vários perfis recebe todas.
    /// As listas detalhadas vêm de GET /api/clinica/consultas com MarcadasPorMim / AtendidasPorMim.
    /// </summary>
    public class GetMeuPainelDto
    {
        // Meus dados (TECNICO vê só esta parte)
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Formacao { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }
        public string? NomeClinica { get; set; }
        public List<string> Perfis { get; set; } = new();

        /// <summary>SECRETARIO: consultas que ele marcou nesta clínica, por status.</summary>
        public ResumoConsultasDto? ConsultasMarcadas { get; set; }

        /// <summary>VETERINARIO: avaliações que ele preencheu e consultas pendentes da clínica para atender.</summary>
        public ResumoVeterinarioDto? Veterinario { get; set; }
    }

    public class ResumoConsultasDto
    {
        public int Total { get; set; }
        public int Pendentes { get; set; }
        public int EmAndamento { get; set; }
        public int Concluidas { get; set; }
        public int Canceladas { get; set; }
    }

    public class ResumoVeterinarioDto
    {
        public int AvaliacoesPreenchidas { get; set; }
        public int ConsultasConcluidas { get; set; }
        public int ConsultasPendentesNaClinica { get; set; }
    }
}
