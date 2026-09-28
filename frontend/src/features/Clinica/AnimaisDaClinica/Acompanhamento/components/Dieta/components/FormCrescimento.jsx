import React from "react";
import { Form, InputNumber, Row, Col, Select } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

const FormCrescimento = ({ form, perfis, loading, especie }) => {
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
      <div className="mb-3">
        <LabelComInfo
          label={<h4 className="text-sm font-semibold text-gray-700 m-0">Ajuste Específico de Crescimento</h4>}
          tituloPopover="Restrições Literárias (NRC)"
          conteudoPopover={
            <span>
              Esta equação considera filhotes <strong>desmamados</strong>. No caso de filhotes recém-nascidos a necessidade corresponde a cerca de 25 kcal para cada 100 g de PC (Kienzle et al., 1985). Além disso, a necessidade energética de manutenção de filhotes inativos, sem oportunidade ou com pouco estímulo à prática de exercícios físicos pode ser reduzida em 10 a 20%, enquanto para filhotes muito ativos, a necessidade pode ser maior.
            </span>
          }
        />
      </div>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="pesoObjetivo"
            label={
              <LabelComInfo
                label="Peso Adulto Estimado (kg)"
                tituloPopover="Peso Adulto Estimado"
                conteudoPopover="A meta de peso apenas para esta prescrição específica. Se deixado em branco, o sistema usará o Peso Ideal cadastrado."
              />
            }
          >
            <InputNumber
              placeholder="Ex: 17,5"
              style={{ width: "100%" }}
              min={0.1}
              max={300}
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
            label={
              <LabelComInfo
                label="Perfil Metabólico"
                tituloPopover="Perfil Metabólico de Crescimento"
                conteudoPopover="Permite selecionar um perfil pré-cadastrado. Se deixado em branco, o sistema utilizará o cálculo genérico padrão para filhotes em crescimento."
              />
            }
          >
            <Select
              allowClear
              loading={loading}
              options={perfisParaExibir}
              placeholder="Selecione um perfil de crescimento..."
            />
          </Form.Item>
        </Col>
      </Row>

      <FormulaPreview
        tipoDieta={2}
        isGato={isGato}
        isCustomizado={false}
        multiplicadorCustom={multiplicadorCustom}
        potencia={potencia}
        pesoAtual={pesoAtualWatch}
        pesoObjetivo={Form.useWatch("pesoObjetivo", form)}
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

export default FormCrescimento;
