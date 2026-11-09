import React, { useContext, useMemo, useState } from "react";
import { Grid, Table } from "antd";
import { PreencherAlturaContext } from "../layout/PaginaPadrao";
import SeletorColunas from "./SeletorColunas";

// Mesmo limite do breakpoint md do antd e do Tailwind
const ehCelular = () =>
  typeof window !== "undefined" && !!window.matchMedia?.("(max-width: 767px)").matches;

/**
 * TabelaPadrao
 * Padroniza o visual de todas as tabelas (tokens em theme/ant-theme.js e moldura em index.css)
 * e repassa os demais props para o Table do antd.
 *
 * Props próprias:
 * - barra: conteúdo à esquerda da linha acima da tabela (ex.: Input.Search, filtros).
 * - selecionarColunas: mostra o botão "Colunas" à direita da barra. A coluna de key "acoes"
 *   fica sempre visível e fora da lista.
 * - colunasIniciais: keys das colunas visíveis ao abrir (padrão: todas).
 * - colunasCelular: keys das colunas visíveis ao abrir no celular (< 768px). Sem ela, o celular usa
 *   colunasIniciais. A tela é lida só ao abrir; depois o botão "Colunas" libera as outras.
 * - preencherAltura: em telas >= md, a tabela ocupa a altura que sobra no contêiner e as linhas
 *   rolam por dentro, com cabeçalho fixo (sticky) e paginação no rodapé da área. É feito só com
 *   CSS (.tabela-preenche em index.css), sem scroll.y, para o cabeçalho continuar na mesma
 *   <table> e as colunas respeitarem a largura dos títulos. Por padrão é ligado só dentro da
 *   PaginaPadrao (contexto); fora dela (modal, card de painel) a tabela cresce com o conteúdo.
 */
const TabelaPadrao = ({
  barra,
  selecionarColunas = false,
  colunasIniciais,
  colunasCelular,
  preencherAltura,
  columns = [],
  scroll,
  className = "",
  ...props
}) => {
  const dentroDaPagina = useContext(PreencherAlturaContext);
  const telas = Grid.useBreakpoint();
  const ativo = (preencherAltura ?? dentroDaPagina) && !!telas.md;

  // Colunas que o usuário pode esconder (todas, menos "acoes"). Estado só visual.
  const colunasOpcionais = useMemo(
    () => columns.filter((col) => col.key !== "acoes"),
    [columns]
  );
  // Guarda as ocultas (e não as visíveis) para que uma coluna que surja depois (ex.: "Clínica" no
  // modo cópias das rações) já apareça visível.
  const [ocultas, setOcultas] = useState(() => {
    // matchMedia e não o useBreakpoint: no primeiro render o useBreakpoint ainda vem vazio
    const iniciais = colunasCelular && ehCelular() ? colunasCelular : colunasIniciais;
    return iniciais
      ? colunasOpcionais.map((col) => col.key).filter((key) => !iniciais.includes(key))
      : [];
  });
  const visiveis = useMemo(
    () => colunasOpcionais.map((col) => col.key).filter((key) => !ocultas.includes(key)),
    [colunasOpcionais, ocultas]
  );
  const alterarVisiveis = (novasVisiveis) =>
    setOcultas(colunasOpcionais.map((col) => col.key).filter((key) => !novasVisiveis.includes(key)));
  const colunasExibidas = useMemo(
    () =>
      selecionarColunas
        ? columns.filter((col) => col.key === "acoes" || !ocultas.includes(col.key))
        : columns,
    [columns, ocultas, selecionarColunas]
  );

  const temBarra = barra || selecionarColunas;

  return (
    <div className={`flex flex-col gap-3 ${ativo ? "flex-1 min-h-0" : ""}`}>
      {temBarra && (
        <div className="flex flex-wrap items-center gap-3">
          {barra}
          {selecionarColunas && (
            <div className="ml-auto">
              <SeletorColunas
                colunas={colunasOpcionais}
                visiveis={visiveis}
                onAlterar={alterarVisiveis}
              />
            </div>
          )}
        </div>
      )}

      <div className={ativo ? "tabela-preenche flex-1 min-h-0" : undefined}>
        <Table
          className={`tabela-padrao ${className}`.trim()}
          columns={colunasExibidas}
          scroll={scroll}
          {...props}
        />
      </div>
    </div>
  );
};

export default TabelaPadrao;
