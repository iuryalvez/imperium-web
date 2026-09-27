import React from "react";
import { Form, Row, Col, Select, InputNumber, Checkbox, Alert } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

const FormGestacao = ({ form, especie, perfis, loading }) => {
  const perfisParaExibir = perfis || [];
  const isCustomizado = Form.useWatch("isCustomizado", form);
  const multiplicadorCustom = Form.useWatch("multiplicadorEnergeticoCustom", form);
  const potenciaCustom = Form.useWatch("potenciaCustom", form);
  const pesoAtualWatch = Form.useWatch("pesoAtual", form);
  const perfilMetabolicoId = Form.useWatch("perfilMetabolico", form);
  const semanaGestacaoWatch = Form.useWatch("semanaGestacao", form);

  const adicionalGestacaoWatch = Form.useWatch("adicionalGestacao", form);

  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";
  const isCao = especie === 1 || String(especie).toLowerCase() === "cao";

  const perfilSelecionado = perfisParaExibir.find((p) => p.value === perfilMetabolicoId);
  const perfilMetabolicoLabel = perfilSelecionado?.label;

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Gestação
      </h4>

      {isGato && (
        <Alert
          message="Recomendação NRC (2006) para Gatas"
          description="Gatas gestantes não possuem equação NRC definida. A literatura recomenda alimentação ad libitum ou acompanhamento rigoroso para um ganho de peso constante de 40 a 50% no pré-parto. O sistema sugere calcular a dieta com base em algum perfil de manutenção, porém também é permitido que você ajuste o multiplicador conforme a necessidade."
          type="warning"
          showIcon
          className="mb-4"
        />
      )}

      {isCao && (
        <Alert
          message="Gestação Fase Final (Cadelas)"
          description="O NRC (2006) sugere que na fase final da gestação (> 4 semanas) seja adicionado um fator extra (26 × Peso) à equação base de manutenção escolhida. Você pode marcar a opção abaixo para forçar essa adição em qualquer momento, caso julgue necessário."
          type="info"
          showIcon
          className="mb-4"
        />
      )}

      <Row gutter={16}>
        {isCao && (
          <Col xs={24} md={12}>
            <Form.Item
              name="semanaGestacao"
              label={
                <LabelComInfo
                  label="Semana de Gestação"
                  tituloPopover="Tempo de Gestação"
                  conteudoPopover="Semanas após o acasalamento. Após a 4ª semana, as necessidades energéticas da cadela aumentam consideravelmente."
                />
              }
            >
              <InputNumber
                placeholder="Ex: 5"
                style={{ width: "100%" }}
                min={1}
                max={9}
                step={1}
              />
            </Form.Item>
          </Col>
        )}
      </Row>

      {isCao && (
        <Form.Item name="adicionalGestacao" valuePropName="checked" className="mb-4">
          <Checkbox>
            Adicionar fator extra de Fase Final (+ 26 × Peso) ao cálculo
          </Checkbox>
        </Form.Item>
      )}

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico Base / Fórmula"
                tituloPopover="Perfil Metabólico"
                conteudoPopover="Permite selecionar um perfil pré-cadastrado. Para gestação inicial ou gatas, use perfis de manutenção. Para cadelas em fase final, utilize o perfil de Gestação Final."
              />
            }
          >
            <Select
              allowClear
              loading={loading}
              disabled={isCustomizado}
              options={perfisParaExibir}
              placeholder="Selecione um perfil metabólico ou fórmula..."
            />
          </Form.Item>
        </Col>
      </Row>

      <FormulaPreview
        tipoDieta={4} // Gestacao
        isGato={isGato}
        isCustomizado={isCustomizado}
        multiplicadorCustom={multiplicadorCustom}
        potenciaCustom={potenciaCustom}
        pesoAtual={pesoAtualWatch}
        perfilMetabolicoLabel={perfilMetabolicoLabel}
        adicionalGestacao={adicionalGestacaoWatch}
      />

      <Form.Item name="isCustomizado" valuePropName="checked" className="mb-2 mt-4">
        <Checkbox>
          Desejo personalizar o Multiplicador (K) e a Potência (P)
        </Checkbox>
      </Form.Item>

      {isCustomizado && (
        <Row gutter={16} className="mt-2">
          <Col xs={24} md={12}>
            <Form.Item
              name="multiplicadorEnergeticoCustom"
              label={
                <LabelComInfo
                  label="Multiplicador Customizado (K)"
                  tituloPopover="Fator Customizado"
                  conteudoPopover="Personaliza o multiplicador principal da fórmula."
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
              name="potenciaCustom"
              label={
                <LabelComInfo
                  label="Potência Customizada (P)"
                  tituloPopover="Potência Customizada"
                  conteudoPopover="Sobrescreve a potência (P) da equação. (Geralmente 0,75 para cães e 0,67 para gatos)."
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
      )}
    </div>
  );
};

export default FormGestacao;
