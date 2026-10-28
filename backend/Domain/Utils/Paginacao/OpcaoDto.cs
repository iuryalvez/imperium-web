namespace backend.Domain.Utils.Paginacao
{
    /// <summary>
    /// Item leve para selects com busca remota (endpoints /opcoes).
    /// Itens inativos vêm com IsAtivo = false para o frontend exibi-los desabilitados.
    /// </summary>
    public class OpcaoDto
    {
        public const int LimiteOpcoes = 20;

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool IsAtivo { get; set; } = true;
    }
}
