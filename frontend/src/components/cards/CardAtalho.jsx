import React from "react";
import { Button, Card, Statistic } from "antd";
import { ArrowRightOutlined } from "@ant-design/icons";
import IconeMascara from "../atoms/IconeMascara";

/**
 * CardAtalho
 * Card de grade que leva a outra tela: ícone, título, um valor ou uma frase e o link no rodapé.
 * Usado na Visão Geral da Clínica (com números) e no início do admin (com descrição).
 *
 * Props:
 * - titulo, icone (PNG de assets/), rotulo (texto do link) e onClick (navegação).
 * - valor: número/texto em destaque (Statistic). Sem valor, o título vira o destaque.
 * - descricao: frase curta abaixo do valor/título.
 *
 * O card inteiro é clicável (mouse, Enter e espaço). O "Acessar →" é só a indicação visual.
 */
const CardAtalho = ({ titulo, icone, valor, descricao, rotulo = "Acessar", onClick }) => (
  <Card
    hoverable
    role="link"
    tabIndex={0}
    aria-label={`${rotulo}: ${titulo}`}
    onClick={onClick}
    onKeyDown={(e) => {
      if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        onClick?.();
      }
    }}
    className="h-full shadow-sm hover:shadow-md transition-shadow cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-verde-claro"
    styles={{ body: { height: "100%", display: "flex", flexDirection: "column", gap: 8 } }}
  >
    {valor !== undefined ? (
      <Statistic title={titulo} value={valor} prefix={<IconeMascara src={icone} />} />
    ) : (
      <div className="flex items-center gap-3">
        <span className="flex items-center justify-center w-10 h-10 rounded-full bg-verde-fraco">
          <IconeMascara src={icone} />
        </span>
        <h3 className="m-0 text-lg font-semibold text-verde-escuro">{titulo}</h3>
      </div>
    )}
    {descricao && <p className="m-0 text-sm text-gray-500">{descricao}</p>}
    <Button type="link" tabIndex={-1} className="self-start p-0 mt-auto">
      {rotulo} <ArrowRightOutlined />
    </Button>
  </Card>
);

export default CardAtalho;
