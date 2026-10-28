namespace backend.Domain.Utils.Paginacao
{
    /// <summary>
    /// Resposta padrão das listagens paginadas: { itens, total, pagina, tamanhoPagina }.
    /// </summary>
    public class PaginaResultado<T>
    {
        public List<T> Itens { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }

        /// <summary>Converte os itens (ex.: entidade → DTO) mantendo os dados de paginação.</summary>
        public PaginaResultado<TDestino> Converter<TDestino>(Func<IEnumerable<T>, IEnumerable<TDestino>> conversor)
        {
            return new PaginaResultado<TDestino>
            {
                Itens = conversor(Itens).ToList(),
                Total = Total,
                Pagina = Pagina,
                TamanhoPagina = TamanhoPagina
            };
        }
    }
}
