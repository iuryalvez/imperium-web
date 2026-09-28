import React from "react";
import { Form, Row, Col, Select, InputNumber, Alert, Collapse, List, Checkbox } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

const FormGestacao = ({ form, especie, perfis, loading }) => {
  const perfisParaExibir = perfis || [];
  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";
  const isCao = especie === 1 || String(especie).toLowerCase() === "cao";
  const multiplicadorCustom = Form.useWatch("multiplicadorEnergetico", form);
  const potencia = Form.useWatch("potencia", form);
  const pesoAtualWatch = Form.useWatch("pesoAtual", form);
  const perfilMetabolicoId = Form.useWatch("perfilMetabolico", form);
  const hasAdicionalGestacaoCaoWatch = Form.useWatch("hasAdicionalGestacaoCao", form);

  const perfilSelecionado = perfisParaExibir.find((p) => p.value === perfilMetabolicoId);
  const perfilMetabolicoLabel = perfilSelecionado?.label;

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Gestação
      </h4>

      {isGato && (
        <Alert
          message="Recomendação NRC (2006) para Gatas (Customização Obrigatória)"
          description="Gatas gestantes não possuem equação NRC definida. A literatura recomenda alimentação ad libitum ou acompanhamento rigoroso para um ganho de peso constante de 40 a 50% no pré-parto. Portanto, o preenchimento do K e do P é obrigatório para Gestação de Gatas."
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
        <Form.Item name="hasAdicionalGestacaoCao" valuePropName="checked" className="mb-4">
          <Checkbox>
            Adicionar fator extra de Fase Final (+ 26 × Peso) ao cálculo
          </Checkbox>
        </Form.Item>
      )}

      {isCao && (
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="perfilMetabolico"
              label={
                <LabelComInfo
                  label="Perfil Metabólico"
                  tituloPopover="Perfil Metabólico"
                  conteudoPopover="Permite selecionar um perfil pré-cadastrado. Para gestação inicial ou gatas, use perfis de manutenção. Para cadelas em fase final, utilize o perfil de Gestação Final."
                />
              }
            >
              <Select
                allowClear
                loading={loading}
                options={perfisParaExibir}
                placeholder="Selecione um perfil metabólico ou fórmula..."
              />
            </Form.Item>
          </Col>
        </Row>
      )}

      {isGato && (
        <Collapse
          size="small"
          className="mb-4 bg-white"
          items={[
            {
              key: '1',
              label: 'Perfis Metabólicos de Manutenção Conhecidos',
              children: (
                <div className="max-h-60 overflow-y-auto">
                  <List
                    size="small"
                    dataSource={perfisParaExibir.filter(
                      (p) =>
                        p.tiposDieta?.includes(1) &&
                        p.label.toLowerCase().includes("gato"),
                    )}
                    renderItem={(item) => (
                      <List.Item>
                        <List.Item.Meta
                          title={
                            <span className="text-sm text-gray-700">
                              {item.label}
                            </span>
                          }
                        />
                      </List.Item>
                    )}
                  />
                </div>
              ),
            },
          ]}
        />
      )}

      <FormulaPreview
        tipoDieta={4} // Gestacao
        isGato={isGato}
        isCustomizado={false}
        multiplicadorCustom={multiplicadorCustom}
        potencia={potencia}
        pesoAtual={pesoAtualWatch}
        perfilMetabolicoLabel={perfilMetabolicoLabel}
        hasAdicionalGestacaoCao={hasAdicionalGestacaoCaoWatch}
      />

      <Row gutter={16} className="mt-4">
        <Col xs={24} md={12}>
            <Form.Item
              name="multiplicadorEnergetico"
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
              name="potencia"
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
    </div>
  );
};

export default FormGestacao;
