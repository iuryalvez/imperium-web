// src/pages/Acompanhamento/components/DetalhesDieta.jsx

import React from "react";
import {
  Statistic,
  Row,
  Col,
  Descriptions,
  Typography,
  Divider,
  Tag,
  Popover,
} from "antd";
import {
  ExperimentOutlined,
  CalculatorOutlined,
  InfoCircleOutlined,
} from "@ant-design/icons";
import FormulaPreview from "./components/FormulaPreview";

const { Text } = Typography;

// Conteúdo literário formatado com HTML nativo (sem sintaxe LaTeX que quebra o JSX)
const conteudoTodasFormulas = (
  <div className="max-w-md max-h-96 overflow-y-auto space-y-3 text-xs leading-relaxed text-gray-700 p-1">
    <div>
      <p className="font-bold text-[var(--verde-escuro)] mb-1">
        CÃES (NRC, 2006)
      </p>
      <ul className="list-disc pl-4 space-y-1">
        <li>
          <strong>Inativos:</strong> 95 × (PC em kg)<sup>0,75</sup>
        </li>
        <li>
          <strong>Ativos ou de canil (Média):</strong> 130 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Adultos jovens e ativos:</strong> 140 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Terriers adultos e ativos:</strong> 180 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Dogue Alemão adulto e ativo:</strong> 200 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Idosos ativos / Newfoundlands:</strong> 105 × (PC em kg)
          <sup>0,75</sup>
        </li>
      </ul>
    </div>

    <div className="border-t pt-2">
      <p className="font-bold text-[var(--verde-escuro)] mb-1">
        GATOS (NRC, 2006)
      </p>
      <ul className="list-disc pl-4 space-y-1">
        <li>
          <strong>Domésticos magros ou ideais (ECC ≤ 5):</strong> 100 × (PC em
          kg)<sup>0,67</sup>
        </li>
        <li>
          <strong>Domésticos obesos (ECC &gt; 5):</strong> 130 × (PC em kg)
          <sup>0,40</sup>
        </li>
        <li>
          <strong>Gatos exóticos (Média):</strong> 157,5 × (PC em kg)
          <sup>0,75</sup>
        </li>
      </ul>
    </div>
  </div>
);

const DetalhesDieta = ({ dieta, isGato }) => {
  if (!dieta) return null;

  // Fallbacks de segurança para compatibilidade entre os nomes de propriedades (DTO vs Frontend)
  const kcal =
    dieta.necessidadeEnergeticaKcal ?? dieta.energiaMetabolizavel ?? 0;
  const gramas = dieta.quantidadeAlimentoGramas ?? dieta.qtdAlimento ?? 0;
  const racao = dieta.racaoUtilizada ?? dieta.nomeRacao ?? "Não informada";

  const formatadorDuasCasas = new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  const formatadorUmaCasa = new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 1, maximumFractionDigits: 1 });

  return (
    <div>
      {/* 1. Resultados Principais em Destaque */}
      <Row gutter={[16, 16]}>
        <Col xs={24} md={12}>
          <div className="flex items-center gap-1.5 mb-1">
            <span className="text-gray-500 text-xs uppercase tracking-wider font-semibold">
              Necessidade Energética
            </span>
            <Popover
              title={
                <Text className="font-bold text-[var(--verde-escuro)]">
                  Fórmulas de Necessidade Energética (NRC)
                </Text>
              }
              content={conteudoTodasFormulas}
              trigger="click"
              placement="right"
            >
              <InfoCircleOutlined className="text-[var(--verde-medio)] cursor-pointer hover:text-[var(--verde-escuro)] transition-colors text-sm" />
            </Popover>
          </div>
          <Statistic
            value={kcal}
            formatter={(val) => formatadorDuasCasas.format(val)}
            suffix="kcal/dia"
            valueStyle={{ color: "var(--verde-escuro)", fontWeight: 600 }}
          />
        </Col>
        <Col xs={24} md={12}>
          <Statistic
            title="Quantidade de Alimento"
            value={gramas}
            formatter={(val) => formatadorDuasCasas.format(val)}
            prefix={
              <ExperimentOutlined className="text-[var(--verde-medio)] mr-1" />
            }
            suffix="g/dia"
            valueStyle={{ color: "var(--verde-escuro)", fontWeight: 600 }}
          />
        </Col>
      </Row>

      {/* 2. Transparência do Cálculo (Explainable Algorithm) */}
      {dieta.perfilNutricional && (
        <div className="my-4 p-3 bg-gray-50 border border-gray-200 rounded-md">
          <div className="flex items-center gap-1 text-[var(--verde-escuro)] font-semibold mb-1 text-xs uppercase tracking-wider">
            <CalculatorOutlined />
            <span>Perfil Metabólico</span>
          </div>

          <Text className="block text-sm text-gray-700 font-medium mb-3">
            {dieta.isCustomizado 
              ? "Fórmula Customizada" 
              : (dieta.perfilNutricional || "Cálculo padrão baseado na literatura")}
          </Text>

          <div className="pt-2 border-t border-gray-200 mt-2">
            <FormulaPreview
              tipoDieta={dieta.tipoDieta}
              isGato={isGato}
              isCustomizado={true} // Força true para usar os valores exatos calculados no backend
              multiplicadorCustom={dieta.fatorK}
              potenciaCustom={dieta.expoente}
              pesoAtual={dieta.pesoAtual}
              pesoObjetivo={dieta.pesoObjetivo}
              perfilMetabolicoLabel={dieta.perfilNutricional}
            />
          </div>
        </div>
      )}

      <Divider dashed className="my-3" />

      {/* 3. Dados Gerais da Prescrição */}
      <Descriptions
        column={1}
        size="small"
        bordered
        labelStyle={{
          backgroundColor: "var(--verde-fraco)",
          color: "var(--verde-escuro)",
          fontWeight: 500,
          width: "140px",
        }}
        contentStyle={{ backgroundColor: "#ffffff" }}
      >
        <Descriptions.Item label="Ração Base">{racao}</Descriptions.Item>
        <Descriptions.Item label="Tipo de Dieta">
          {dieta.tipoDietaLabel}
        </Descriptions.Item>
        {dieta.pesoIdeal > 0 && (
          <Descriptions.Item label="Peso Ideal / Alvo">
            {dieta.pesoIdeal} kg
          </Descriptions.Item>
        )}
        <Descriptions.Item label="Observações">
          {dieta.observacoes || "Nenhuma"}
        </Descriptions.Item>
      </Descriptions>

      {/* 4. Sugestão Prática de Refeições */}
      <Divider orientation="left" plain className="mb-2 mt-4">
        <span className="text-xs font-semibold uppercase text-gray-500 tracking-wider">
          Sugestão de Refeições
        </span>
      </Divider>

      <div className="bg-white p-3 border border-gray-100 rounded shadow-sm text-sm flex flex-col gap-1.5 text-gray-700">
        <Text>
          • <strong>2x ao dia:</strong> {formatadorUmaCasa.format(gramas / 2)}g por refeição
        </Text>
        <Text>
          • <strong>3x ao dia:</strong> {formatadorUmaCasa.format(gramas / 3)}g por refeição
        </Text>
        {gramas > 150 && (
          <Text>
            • <strong>4x ao dia:</strong> {formatadorUmaCasa.format(gramas / 4)}g por refeição
          </Text>
        )}
      </div>
    </div>
  );
};

export default DetalhesDieta;
