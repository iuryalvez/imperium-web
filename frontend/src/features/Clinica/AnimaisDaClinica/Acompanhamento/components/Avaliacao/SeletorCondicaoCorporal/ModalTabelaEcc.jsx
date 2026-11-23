import React from "react";
import { ProfileOutlined } from "@ant-design/icons";
import ModalPadrao from "../../../../../../../components/modal/ModalPadrao";
import tabelaCao from "../../../../../../../assets/ecc/wsava-ecc-cao.webp";
import tabelaGato from "../../../../../../../assets/ecc/wsava-ecc-gato.webp";
import { DESCRICOES_ECC } from "./descricoesEcc";

// Tabelas oficiais em português do WSAVA Global Nutrition Committee.
const TABELAS = {
  cao: {
    imagem: tabelaCao,
    nome: "Cão",
    pdf: "https://wsava.org/wp-content/uploads/2020/01/Body-Condition-Score-Dogs-Portuguese.pdf",
  },
  gato: {
    imagem: tabelaGato,
    nome: "Gato",
    pdf: "https://wsava.org/wp-content/uploads/2020/01/Body-Condition-Score-Cats-Portuguese.pdf",
  },
};

const ModalTabelaEcc = ({ open, onClose, especie }) => {
  const tabela = TABELAS[especie] ?? TABELAS.cao;
  const descricoes = DESCRICOES_ECC[especie] ?? DESCRICOES_ECC.cao;

  return (
    <ModalPadrao
      open={open}
      onCancel={onClose}
      title={`Escore de Condição Corporal — ${tabela.nome}`}
      subtitle="Tabela de referência da WSAVA (escala de 1 a 9)"
      icon={<ProfileOutlined />}
      modo="detalhes"
      tamanho="lg"
    >
      <a href={tabela.imagem} target="_blank" rel="noreferrer" title="Abrir a imagem em tamanho original">
        <img
          src={tabela.imagem}
          alt={`Tabela WSAVA de escore de condição corporal do ${tabela.nome.toLowerCase()}, escores de 1 a 9`}
          className="w-full h-auto rounded-md border border-gray-200"
        />
      </a>
      <p className="text-xs text-gray-500 mt-2 mb-4">
        Fonte: WSAVA Global Nutrition Committee, Classificação da Condição Corporal.{" "}
        <a href={tabela.pdf} target="_blank" rel="noreferrer">
          Ver o PDF original
        </a>
      </p>

      <h4 className="text-sm font-semibold text-verde-escuro mb-2">Descrição de cada escore</h4>
      <dl className="text-xs leading-relaxed text-gray-600 space-y-2 m-0">
        {Object.entries(descricoes).map(([escore, item]) => (
          <div key={escore}>
            <dt className="font-semibold text-gray-800 inline">
              {escore} · {item.classe}
              {item.detalhe ? ` (${item.detalhe})` : ""}:{" "}
            </dt>
            <dd className="inline m-0">{item.texto}</dd>
          </div>
        ))}
      </dl>
    </ModalPadrao>
  );
};

export default ModalTabelaEcc;
