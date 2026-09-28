import React from "react";
import { Form, Row, Col, Select, InputNumber } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

const FormManutencao = ({ form, especie, perfis, loading, tipoDietaSelecionado }) => {
  const perfisParaExibir = perfis || [];
  const multiplicadorCustom = Form.useWatch("multiplicadorEnergetico", form);
  const potencia = Form.useWatch("potencia", form);
  const pesoAtualWatch = Form.useWatch("pesoAtual", form);
  const perfilMetabolicoId = Form.useWatch("perfilMetabolico", form);

  const perfilSelecionado = perfisParaExibir.find((p) => p.value === perfilMetabolicoId);
  const perfilMetabolicoLabel = perfilSelecionado?.label;

  const isGato =
    especie === 2 || String(especie).toLowerCase() === "gato";

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        {tipoDietaSelecionado === 5 ? "Ajuste Específico de Emagrecimento" : "Ajuste Específico de Manutenção"}
      </h4>

      <Row gutter={16}>
        <Col span={tipoDietaSelecionado === 5 ? 12 : 24}>
          <Form.Item
            name="pesoIdeal"
            label={
              <LabelComInfo
                label="Peso Ideal / Alvo (kg)"
                tituloPopover="Peso Referência / Ideal"
                conteudoPopover="Se preenchido, será a base para os cálculos energéticos, sobrescrevendo a média da raça."
              />
            }
          >
            <InputNumber
              placeholder="Ex: 15,0"
              style={{ width: "100%" }}
              min={0.1}
              max={300}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>

        {tipoDietaSelecionado === 5 && (
          <Col span={12}>
            <Form.Item
              name="pesoObjetivo"
              label={
                <LabelComInfo
                  label="Peso Objetivo (kg)"
                  tituloPopover="Peso Objetivo (Emagrecimento)"
                  conteudoPopover="Peso que se deseja atingir a curto/médio prazo durante o processo de emagrecimento."
                />
              }
            >
              <InputNumber
                placeholder="Ex: 12,0"
                style={{ width: "100%" }}
                min={0.01}
                max={300}
                step={0.01}
                decimalSeparator=","
              />
            </Form.Item>
          </Col>
        )}
      </Row>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico (Opcional)"
                tituloPopover="Perfil Metabólico"
                conteudoPopover="Permite selecionar um perfil pré-cadastrado. Se deixado em branco, o sistema utilizará o cálculo genérico padrão para a espécie selecionada."
              />
            }
          >
            <Select
              allowClear
              loading={loading}
              options={perfisParaExibir}
              placeholder="Selecione um perfil metabólico..."
            />
          </Form.Item>
        </Col>
      </Row>

      <FormulaPreview
        tipoDieta={tipoDietaSelecionado || 1}
        isGato={isGato}
        isCustomizado={false} // Mantido prop mas não controla mais nada, FormulaPreview usa multiplicadorCustom
        multiplicadorCustom={multiplicadorCustom}
        potencia={potencia}
        pesoAtual={pesoAtualWatch}
        perfilMetabolicoLabel={perfilMetabolicoLabel}
      />

      <Row gutter={16} className="mt-4">
        <Col xs={24} md={12}>
            <Form.Item
              name="multiplicadorEnergetico"
              label={
                <LabelComInfo
                  label="Multiplicador Customizado (K)"
                  tituloPopover="Fator Customizado"
                  conteudoPopover="Personaliza o multiplicador K na fórmula. Ex: Para gatos exóticos pode variar de 55 a 260."
                />
              }
              rules={[{ required: true, message: "Obrigatório" }]}
            >
              <InputNumber
                placeholder="Ex: 130"
                style={{ width: "100%" }}
                min={1}
                max={1000}
                step={0.5}
                decimalSeparator=","
              />
            </Form.Item>
          </Col>
          <Col xs={24} md={12}>
            <Form.Item
              name="potencia"
              label={
                <LabelComInfo
                  label="Potência Customizada (P)"
                  tituloPopover="Potência Customizada"
                  conteudoPopover="Sobrescreve a potência P da fórmula selecionada (Geralmente 0,75 para cães ou 0,67 para gatos)."
                />
              }
              rules={[{ required: true, message: "Obrigatório" }]}
            >
              <InputNumber
                placeholder="Ex: 0,75"
                style={{ width: "100%" }}
                min={0.1}
                max={2}
                step={0.01}
                decimalSeparator=","
              />
            </Form.Item>
          </Col>
      </Row>
    </div>
  );
};

export default FormManutencao;
