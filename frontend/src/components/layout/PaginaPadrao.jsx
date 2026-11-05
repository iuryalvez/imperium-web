import React, { createContext } from "react";
import { Breadcrumb } from "antd";
import { Link } from "react-router-dom";

// Avisa a TabelaPadrao de que ela está numa área de altura definida e pode preencher o espaço.
// O ModalPadrao volta para false, para tabelas dentro de modal não herdarem isso.
export const PreencherAlturaContext = createContext(false);

/**
 * PaginaPadrao
 * Esqueleto das telas de lista: cabeçalho (título + ações), linha de filtros e o conteúdo.
 * Em telas >= md ocupa toda a altura da área de conteúdo, e o conteúdo (em geral a TabelaPadrao)
 * fica com o espaço que sobra e rola por dentro. Abaixo de md (celular) a página rola normalmente.
 *
 * Props:
 * - titulo: texto do h1.
 * - descricao: subtítulo opcional abaixo do h1.
 * - trilha: breadcrumb acima do título, lista de { titulo, para } (ex.: [{ titulo: "Início", para: "/sistema" }]).
 *   O último nível é a própria página (o título) e entra sem link.
 * - acoes: botões do cabeçalho (ex.: "Cadastrar").
 * - filtros: busca e filtros acima do conteúdo (ex.: Input.Search).
 * - children: o conteúdo principal.
 * - preencher (padrão true): false nas telas de vários cards (Meu Painel, perfil, início do admin).
 *   O conteúdo fica com a altura natural e quem rola é a área de conteúdo do shell.
 */
const PaginaPadrao = ({ titulo, descricao, trilha, acoes, filtros, preencher = true, children }) => {
  return (
    <div className={`flex flex-col gap-4 ${preencher ? "flex-1 md:min-h-0" : ""}`}>
      {trilha?.length > 0 && (
        <Breadcrumb
          className="-mb-2"
          items={[
            ...trilha.map((nivel) => ({ title: <Link to={nivel.para}>{nivel.titulo}</Link> })),
            { title: titulo },
          ]}
        />
      )}

      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="min-w-0">
          <h1 className="m-0 text-2xl font-bold text-verde-medio">{titulo}</h1>
          {descricao && <p className="m-0 mt-1 text-sm text-gray-500">{descricao}</p>}
        </div>
        {acoes && <div className="flex flex-wrap gap-2">{acoes}</div>}
      </div>

      {filtros && (
        <div className="flex flex-wrap items-center gap-3">{filtros}</div>
      )}

      <div className={`flex flex-col ${preencher ? "flex-1 md:min-h-0" : "gap-6"}`}>
        <PreencherAlturaContext.Provider value={preencher}>{children}</PreencherAlturaContext.Provider>
      </div>
    </div>
  );
};

export default PaginaPadrao;
