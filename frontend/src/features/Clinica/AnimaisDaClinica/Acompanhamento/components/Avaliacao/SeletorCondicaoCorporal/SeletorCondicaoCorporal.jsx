import React, { useState } from "react";
import { Button, ConfigProvider, Slider } from "antd";
import { InfoCircleOutlined } from "@ant-design/icons";
import { colors } from "../../../../../../../theme/colors";
import SilhuetaEcc from "./SilhuetaEcc";
import ModalTabelaEcc from "./ModalTabelaEcc";
import { descricaoEcc } from "./descricoesEcc";
import { ESCORES_SILHUETA } from "./geometriaSilhuetas";

const { extremo, atencao, ideal } = colors.ecc;

// Faixa verde do trilho por espécie: no cão 4 e 5 são ideais; no gato, só o 5.
// Posição do escore no trilho: (escore - 1) / 8 → 1 = 0%, 5 = 50%, 9 = 100%.
const GRADIENTES = {
  cao: `linear-gradient(to right, ${extremo} 0%, ${extremo} 6%, ${atencao} 22%, ${atencao} 30%, ${ideal} 37.5%, ${ideal} 50%, ${atencao} 62.5%, ${atencao} 72%, ${extremo} 94%, ${extremo} 100%)`,
  gato: `linear-gradient(to right, ${extremo} 0%, ${extremo} 6%, ${atencao} 25%, ${atencao} 40%, ${ideal} 50%, ${atencao} 60%, ${atencao} 72%, ${extremo} 94%, ${extremo} 100%)`,
};

const MARCAS = Object.fromEntries(
  [1, 2, 3, 4, 5, 6, 7, 8, 9].map((n) => [n, { label: <span className="text-xs text-gray-500">{n}</span> }]),
);

// O slider ocupa de 10% a 90% da largura, alinhado ao centro das 5 colunas de silhuetas:
// o escore n fica em 10% + (n - 1) × 10%.
const posicaoEscore = (escore) => `${10 + (escore - 1) * 10}%`;

// Escore ímpar destaca a própria silhueta; par destaca as duas vizinhas (está entre elas).
export const silhuetasDestacadas = (valor) => {
  if (valor == null) return [];
  return valor % 2 === 1 ? [valor] : [valor - 1, valor + 1];
};

/**
 * Rótulo do campo de ECC: nome, ⓘ que abre a tabela WSAVA da espécie e, quando
 * `onLimpar` é passado, o botão "Limpar" (ECC opcional, no cão).
 */
export const RotuloCondicaoCorporal = ({ especie, onLimpar }) => {
  const [tabelaAberta, setTabelaAberta] = useState(false);

  return (
    <span className="flex items-center gap-2">
      <span>Condição Corporal (ECC · 1 a 9)</span>
      <InfoCircleOutlined
        role="button"
        tabIndex={0}
        aria-label="Ver tabela de condição corporal da WSAVA"
        className="text-azul-info cursor-pointer hover:text-azul-hover transition-colors"
        onClick={(e) => {
          e.preventDefault();
          setTabelaAberta(true);
        }}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            setTabelaAberta(true);
          }
        }}
      />
      {onLimpar && (
        <Button
          type="link"
          size="small"
          className="text-xs p-0 m-0 h-auto"
          onClick={(e) => {
            e.preventDefault();
            onLimpar();
          }}
        >
          Limpar
        </Button>
      )}
      <ModalTabelaEcc open={tabelaAberta} onClose={() => setTabelaAberta(false)} especie={especie} />
    </span>
  );
};

/**
 * Seletor do escore de condição corporal (1 a 9): silhuetas clicáveis (1, 3, 5, 7 e 9),
 * slider com trilho colorido e balão do valor, e a descrição do escore escolhido.
 * Compatível com Form.Item (`value` / `onChange`).
 */
const SeletorCondicaoCorporal = ({ id, value, onChange, especie = "cao" }) => {
  const valor = value ?? null;
  const destacadas = silhuetasDestacadas(valor);
  const descricao = valor != null ? descricaoEcc(especie, valor) : null;

  return (
    <div id={id} className="select-none">
      <div className="grid grid-cols-5">
        {ESCORES_SILHUETA.map((escore) => {
          const ativa = destacadas.includes(escore);
          return (
            <button
              key={escore}
              type="button"
              aria-label={`Escore ${escore}`}
              aria-pressed={valor === escore}
              title={`Escore ${escore}`}
              onClick={() => onChange?.(escore)}
              className={`flex justify-center bg-transparent border-0 p-1 cursor-pointer rounded-md transition-all hover:opacity-100 focus-visible:outline focus-visible:outline-2 focus-visible:outline-azul-info ${
                ativa ? "text-verde-escuro opacity-100" : "text-cinza-escuro opacity-40"
              }`}
            >
              <SilhuetaEcc especie={especie} escore={escore} className="w-full max-w-[96px] h-auto" />
            </button>
          );
        })}
      </div>

      <div className="relative pt-7">
        {valor != null && (
          <span
            className="absolute top-0 -translate-x-1/2 min-w-[32px] text-center text-xs font-semibold text-verde-escuro bg-white border border-solid border-verde-claro rounded-md px-1.5 py-0.5 shadow-sm"
            style={{ left: posicaoEscore(valor) }}
            aria-hidden="true"
          >
            {valor}
            <span className="absolute left-1/2 -bottom-[5px] -translate-x-1/2 w-2 h-2 rotate-45 bg-white border-r border-b border-solid border-verde-claro border-t-0 border-l-0" />
          </span>
        )}
        <ConfigProvider theme={{ components: { Slider: { railSize: 8, handleSize: 14, handleSizeHover: 16 } } }}>
          <Slider
            min={1}
            max={9}
            step={1}
            marks={MARCAS}
            dotStyle={{ display: "none" }}
            activeDotStyle={{ display: "none" }}
            value={valor ?? undefined}
            onChange={(novo) => onChange?.(novo)}
            tooltip={{ open: false }}
            aria-label="Escore de condição corporal"
            style={{ marginInline: "10%" }}
            styles={{
              rail: { background: GRADIENTES[especie] ?? GRADIENTES.cao, borderRadius: 999 },
              track: { background: "transparent" },
              handle: valor == null ? { display: "none" } : undefined,
            }}
          />
        </ConfigProvider>
      </div>

      <p className="text-xs leading-relaxed text-gray-600 mt-6 mb-0 min-h-[2.5em]" aria-live="polite">
        {descricao ? (
          <>
            <strong className="text-verde-escuro">
              {valor} · {descricao.classe}
              {descricao.detalhe ? ` (${descricao.detalhe})` : ""}
            </strong>
            {" — "}
            {descricao.texto}
          </>
        ) : (
          <span className="italic">Não informado. Clique numa figura ou no controle para escolher o escore.</span>
        )}
      </p>
    </div>
  );
};

export default SeletorCondicaoCorporal;
