using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.Utils.Paginacao
{
    /// <summary>
    /// Helpers de IQueryable para ordenar e paginar no banco (Skip/Take), evitando trazer a tabela inteira.
    /// </summary>
    public static class QueryablePaginacaoExtensions
    {
        /// <summary>
        /// Ordena pelo campo pedido em filtro.OrdenarPor, desde que ele esteja na lista branca <paramref name="colunas"/>.
        /// Campo ausente ou desconhecido usa <paramref name="colunaPadrao"/>. O Id é usado como desempate para a paginação ser estável.
        /// </summary>
        public static IQueryable<T> Ordenar<T>(
            this IQueryable<T> query,
            FiltroBase filtro,
            IReadOnlyDictionary<string, Expression<Func<T, object?>>> colunas,
            string colunaPadrao,
            Expression<Func<T, int>> desempate)
        {
            var chave = filtro.OrdenarPor != null && colunas.ContainsKey(filtro.OrdenarPor)
                ? filtro.OrdenarPor
                : colunaPadrao;
            var coluna = colunas[chave];

            return filtro.OrdemDescendente
                ? query.OrderByDescending(coluna).ThenByDescending(desempate)
                : query.OrderBy(coluna).ThenBy(desempate);
        }

        /// <summary>Conta o total e busca só a página pedida.</summary>
        public static async Task<PaginaResultado<T>> PaginarAsync<T>(this IQueryable<T> query, FiltroBase filtro)
        {
            var total = await query.CountAsync();
            var itens = await query
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();

            return new PaginaResultado<T>
            {
                Itens = itens,
                Total = total,
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            };
        }

        /// <summary>Monta o padrão de busca para ILIKE (contém, sem diferenciar maiúsculas).</summary>
        public static string PadraoBusca(string busca) => $"%{busca.Replace("%", "\\%").Replace("_", "\\_")}%";
    }
}
