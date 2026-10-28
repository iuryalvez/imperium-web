import { useCallback, useEffect, useRef, useState } from "react";
import { message } from "antd";

const ATRASO_BUSCA_MS = 400;
export const TAMANHOS_PAGINA = [10, 20, 50, 100];

/**
 * Hook para listas paginadas no servidor (contrato FiltroBase / PaginaResultado do backend).
 * A API recebe { pagina, tamanhoPagina, busca, ordenarPor, ordem, ...filtrosDaEntidade }
 * e devolve { itens, total, pagina, tamanhoPagina }.
 *
 * @param {(filtro: object) => Promise<{itens: Array, total: number}>} listar - função da camada api/.
 * @param {object} [filtrosIniciais] - filtros da entidade (ex.: { ativo: true, especie: 1 }).
 * @param {(filtersDaTabela: object) => object} [traduzirFiltros] - converte os filtros de coluna
 *   da Table do antd em filtros do servidor (ex.: { isAtivo: [true] } → { ativo: true }).
 * @returns {{ dados, total, loading, filtros, paginacao, setFiltros, setBusca, handleTableChange, recarregar }}
 */
export const usePaginacaoServidor = (listar, filtrosIniciais = {}, traduzirFiltros) => {
  const [filtros, setFiltrosState] = useState({ pagina: 1, tamanhoPagina: 10, ...filtrosIniciais });
  const [dados, setDados] = useState([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const temporizador = useRef(null);

  const carregar = useCallback(async () => {
    setLoading(true);
    try {
      const resultado = await listar(filtros);
      setDados(resultado?.itens || []);
      setTotal(resultado?.total || 0);
    } catch (error) {
      message.error(error?.message || "Erro ao carregar a lista.");
      setDados([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [listar, filtros]);

  useEffect(() => {
    carregar();
  }, [carregar]);

  useEffect(() => () => clearTimeout(temporizador.current), []);

  /** Mescla filtros novos e volta para a página 1. Valores vazios removem o filtro. */
  const setFiltros = useCallback((novos) => {
    setFiltrosState((atual) => {
      const proximo = { ...atual, ...novos, pagina: 1 };
      Object.keys(proximo).forEach((chave) => {
        if (proximo[chave] === undefined || proximo[chave] === null || proximo[chave] === "") delete proximo[chave];
      });
      return proximo;
    });
  }, []);

  /** Busca por texto com debounce. */
  const setBusca = useCallback(
    (texto) => {
      clearTimeout(temporizador.current);
      temporizador.current = setTimeout(() => setFiltros({ busca: texto }), ATRASO_BUSCA_MS);
    },
    [setFiltros],
  );

  /** onChange da Table do antd: página, tamanho, ordenação e filtros de coluna. */
  const handleTableChange = useCallback(
    (pagination, filtersDaTabela, sorter) => {
      const ordenacao = Array.isArray(sorter) ? sorter[0] : sorter;
      const ordenarPor = ordenacao?.order ? ordenacao.columnKey || ordenacao.field : undefined;
      const extras = traduzirFiltros ? traduzirFiltros(filtersDaTabela || {}) : {};

      setFiltrosState((atual) => {
        const mudouFiltro = JSON.stringify(extras) !== JSON.stringify(
          Object.fromEntries(Object.keys(extras).map((k) => [k, atual[k]])),
        );
        const proximo = {
          ...atual,
          ...extras,
          pagina: mudouFiltro ? 1 : pagination.current,
          tamanhoPagina: pagination.pageSize,
          ordenarPor,
          ordem: ordenacao?.order === "ascend" ? "asc" : ordenacao?.order === "descend" ? "desc" : undefined,
        };
        Object.keys(proximo).forEach((chave) => {
          if (proximo[chave] === undefined || proximo[chave] === null || proximo[chave] === "") delete proximo[chave];
        });
        return proximo;
      });
    },
    [traduzirFiltros],
  );

  const paginacao = {
    current: filtros.pagina,
    pageSize: filtros.tamanhoPagina,
    total,
    showSizeChanger: true,
    pageSizeOptions: TAMANHOS_PAGINA,
    showTotal: (totalItens) => `${totalItens} registro(s)`,
  };

  return { dados, total, loading, filtros, paginacao, setFiltros, setBusca, handleTableChange, recarregar: carregar };
};
