using AutoMapper;

namespace backend.Domain.Utils
{
    /// <summary>
    /// Extensões de AutoMapper usadas pelos MappingProfiles.
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Atualização parcial: um campo nulo no DTO de origem mantém o valor atual da entidade.
        /// A checagem olha a propriedade do DTO (PreCondition). Com
        /// <c>ForAllMembers(opts =&gt; opts.Condition((src, dest, srcMember) =&gt; srcMember != null))</c>,
        /// um <c>int?</c>/<c>bool?</c>/enum/<c>DateTime?</c> nulo chegava já convertido para 0/false/MinValue e
        /// sobrescrevia o valor (ex.: espécie 0 no animal, ração com cálculo quebrado).
        /// Use sempre como a última chamada da configuração do mapa.
        /// </summary>
        public static void IgnorarNulosDaOrigem<TOrigem, TDestino>(this IMappingExpression<TOrigem, TDestino> mapa)
        {
            mapa.ForAllMembers(opts =>
            {
                var propriedadeOrigem = typeof(TOrigem).GetProperty(opts.DestinationMember.Name);
                if (propriedadeOrigem != null)
                {
                    opts.PreCondition((TOrigem origem) => propriedadeOrigem.GetValue(origem) != null);
                }
            });
        }
    }
}
