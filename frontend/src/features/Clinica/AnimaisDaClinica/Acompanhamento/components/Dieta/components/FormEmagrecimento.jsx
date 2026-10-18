import React from "react";
import { Form, Row, Col, Select, InputNumber, Radio } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

const FormEmagrecimento = ({ form, especie, perfis, loading, niveisRestricao, pesoAtual }) => {
  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";
  // eslint-disable-next-line
  const isCao = especie === 1 || String(especie).toLowerCase() === "cao";
  const multiplicadorCustom = Form.useWatch("multiplicadorEnergetico", form);
  const potencia = Form.useWatch("potencia", form);
  const pesoAtualWatch = Form.useWatch("pesoAtual", form);
  const perfilMetabolicoId = Form.useWatch("perfilMetabolico", form);
  const nivelRestricaoWatch = Form.useWatch("nivelRestricao", form);

  const perfisParaExibir = (perfis || []).filter(p => p.tiposDieta?.includes(5));
  
  const perfilSelecionado = perfisParaExibir.find((p) => p.value === perfilMetabolicoId);

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Plano de Emagrecimento
      </h4>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico Base"
                tituloPopover="Perfil Metabólico Base"
                conteudoPopover="Selecione o perfil metabólico de manutenção do animal. O cálculo aplicará uma restrição sobre a energia de manutenção deste perfil."
              />
            }
            rules={[{ required: true, message: "Obrigatório" }]}
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

      <Row gutter={16} className="mt-2">
        <Col xs={24} md={12}>
            <Form.Item
              name="multiplicadorEnergetico"
              label={
                <LabelComInfo
                  label="Multiplicador Customizado (K)"
                  tituloPopover="Fator Customizado"
                  conteudoPopover="Personaliza o multiplicador principal da fórmula base."
                />
              }
              rules={[{ required: true, message: "Obrigatório" }]}
            >
              <InputNumber
                placeholder="Ex: 100"
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
                  conteudoPopover="Sobrescreve a potência (P) da equação base."
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

      <Row gutter={16} className="mt-2">
        <Col xs={24} md={12}>
          <Form.Item
            name="pesoObjetivo"
            label={
              <LabelComInfo
                label="Peso Alvo (kg)"
                tituloPopover="Peso Alvo"
                conteudoPopover="O peso ideal que se deseja atingir no final do plano de emagrecimento."
              />
            }
            rules={[
              { required: true, message: "Obrigatório" },
              {
                validator: (_, value) => {
                  if (value && pesoAtual && value >= pesoAtual) {
                    return Promise.reject(new Error("Peso alvo deve ser menor que o atual"));
                  }
                  return Promise.resolve();
                }
              }
            ]}
          >
            <InputNumber
              placeholder="Ex: 10,5"
              style={{ width: "100%" }}
              min={0.1}
              step={0.1}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="mt-4 mb-4">
        <Form.Item
          name="nivelRestricao"
          label={
            <LabelComInfo
              label="Nível de Restrição"
              tituloPopover="Nível de Restrição"
              conteudoPopover="Define a agressividade da dieta de emagrecimento."
            />
          }
          rules={[{ required: true, message: "Selecione o nível de restrição" }]}
        >
          <Radio.Group className="w-full flex flex-col gap-3">
            {niveisRestricao?.map((nivel) => (
              <Radio key={nivel.value} value={nivel.value}>
                <div className="inline-flex flex-col">
                  <span className="font-medium text-gray-800">{nivel.label} ({(nivel.fator * 100).toFixed(0)}% da EM Base)</span>
                  <span className="text-xs text-gray-500">
                    Estimativa de perda: {(nivel.percentualPerdaSemanal * 100).toFixed(1)}% do peso por semana
                  </span>
                </div>
              </Radio>
            ))}
          </Radio.Group>
        </Form.Item>
      </div>

      <FormulaPreview
        tipoDieta={5}
        isGato={isGato}
        multiplicadorCustom={multiplicadorCustom}
        potencia={potencia}
        pesoAtual={pesoAtualWatch || pesoAtual}
        pesoObjetivo={Form.useWatch("pesoObjetivo", form)}
        perfilMetabolicoLabel={perfilSelecionado?.label}
        fatorRestricao={niveisRestricao?.find(n => n.value === nivelRestricaoWatch)?.fator}
      />
    </div>
  );
};

export default FormEmagrecimento;
