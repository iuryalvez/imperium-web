namespace backend.Domain.Utils.Paginacao
{
    /// <summary>
    /// Parâmetros comuns de paginação, busca e ordenação, recebidos via query string ([FromQuery]).
    /// Cada entidade cria seu filtro herdando desta classe (ex.: AnimalFiltro) e acrescenta os campos próprios.
    /// </summary>
    public class FiltroBase
    {
        public const int TamanhoPaginaPadrao = 10;
        public const int TamanhoPaginaMaximo = 100;

        private int _pagina = 1;
        private int _tamanhoPagina = TamanhoPaginaPadrao;

        /// <summary>Página atual, começando em 1.</summary>
        public int Pagina
        {
            get => _pagina;
            set => _pagina = value < 1 ? 1 : value;
        }

        /// <summary>Itens por página (padrão 10, máximo 100; valores maiores são limitados).</summary>
        public int TamanhoPagina
        {
            get => _tamanhoPagina;
            set => _tamanhoPagina = value < 1 ? TamanhoPaginaPadrao : Math.Min(value, TamanhoPaginaMaximo);
        }

        /// <summary>Texto livre; cada entidade define em quais campos procura.</summary>
        public string? Busca { get; set; }

        /// <summary>Nome do campo de ordenação. Só são aceitos os nomes da lista branca de cada entidade.</summary>
        public string? OrdenarPor { get; set; }

        /// <summary>"asc" ou "desc" (padrão "desc": mais recentes primeiro).</summary>
        public string? Ordem { get; set; }

        public bool OrdemDescendente => !string.Equals(Ordem, "asc", StringComparison.OrdinalIgnoreCase);

        /// <summary>Busca sem espaços nas pontas, ou nulo quando vazia.</summary>
        public string? BuscaNormalizada => string.IsNullOrWhiteSpace(Busca) ? null : Busca.Trim();
    }
}
