import React from "react";
import { Form, InputNumber, Row, Col, Select } from "antd";

// Constantes mapeadas estritamente com o Enum PerfilMetabolico do C#
const perfisCaes = [
  {
    value: "CaoAtivo",
    label: "Cães ativos ou de canil (k=130, exp=0.75)",
  },
  {
    value: "CaoAdultoJovemAtivo",
    label: "Cães adultos jovens e ativos (k=140, exp=0.75)",
  },
  {
    value: "CaoJovemModerado",
    label: "Cães jovens com atividade moderada (k=130, exp=0.75)",
  },
  {
    value: "CaoDogueAlemaoAdultoAtivo",
    label: "Cães Dogue Alemão adultos e ativos (k=200, exp=0.75)",
  },
  {
    value: "CaoTerrierAdultoAtivo",
    label: "Cães Terriers adultos e ativos (k=180, exp=0.75)",
  },
  {
    value: "CaoIdosoAtivo",
    label: "Cães idosos ativos ou Newfoundlands (k=105, exp=0.75)",
  },
  {
    value: "CaoInativo",
    label: "Cães inativos (Necessidade abaixo da média) (k=95, exp=0.75)",
  },
];

const perfisGatos = [
  {
    value: "GatoExotico",
    label: "Gatos exóticos - Média NRC (k=157.5, exp=0.75)",
  },
  {
    value: "GatoDomesticoMagro",
    label:
      "Gatos em adequada condição corporal ou magros - ECC ≤ 5 (k=100, exp=0.67)",
  },
  {
    value: "GatoDomesticoObeso",
    label: "Gatos domésticos obesos - ECC > 5 (k=130, exp=0.40)",
  },
];

const FormManutencao = ({ form, especie }) => {
  // Considera 0 ou 'Gato' para gato, dependendo de como seu backend retorna o Enum de Especie
  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";
  const opcoesMetabolicas = isGato ? perfisGatos : perfisCaes;

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Manutenção
      </h4>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="pesoObjetivo"
            label="Peso Alvo do Ciclo (kg)"
            tooltip="A meta de peso apenas para esta prescrição específica (ex: emagrecimento gradual). Se deixado em branco, o sistema usará o Peso Ideal cadastrado."
          >
            <InputNumber
              placeholder="Ex: 17,5"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label="Perfil Metabólico (Fórmula NRC)"
            tooltip="Opcional. Se deixado em branco, o sistema calculará a fórmula automaticamente com base na espécie e condição corporal."
          >
            <Select
              allowClear
              options={opcoesMetabolicas}
              placeholder="Selecione um perfil para forçar uma fórmula específica..."
            />
          </Form.Item>
        </Col>
      </Row>
    </div>
  );
};

export default FormManutencao;
