import React, { useEffect } from "react";
import { Form, Row, Col, Select, InputNumber, Radio, Alert } from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

export const PERFIL_SUGERIDO_CAO = 8; // Cão Inativo (95 × PC^0,75)
export const PERFIL_SUGERIDO_GATO = 2; // Gato Doméstico - Magro ou Ideal (100 × PC^0,67)
export const LIMITE_PERDA_RECOMENDADA_GATO = 1; // % do peso inicial por semana

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

  // Perfil base sugerido: Cão Inativo (8), para a energia ficar perto da faixa observada pela AAHA,
  // e Gato Doméstico Magro ou Ideal (2), porque a energia é calculada sobre o peso alvo (um peso ideal).
  useEffect(() => {
    if (!perfilMetabolicoId && perfisParaExibir.length > 0) {
      form.setFieldValue("perfilMetabolico", isGato ? PERFIL_SUGERIDO_GATO : PERFIL_SUGERIDO_CAO);
    }
  }, [isGato, perfilMetabolicoId, perfisParaExibir.length, form]);

  const nivelSelecionado = niveisRestricao?.find((n) => n.value === nivelRestricaoWatch);
  const perdaAcimaDoRecomendadoGato =
    isGato && nivelSelecionado && nivelSelecionado.percentualPerdaSemanal * 100 > LIMITE_PERDA_RECOMENDADA_GATO;

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
                conteudoPopover={isGato
                  ? "Perfil de manutenção usado como base. A sugestão é o gato magro ou ideal (100 × PC^0,67), porque a energia é calculada sobre o peso alvo. O cálculo aplica a restrição sobre essa energia."
                  : "Perfil de manutenção usado como base. A sugestão é o cão inativo (95 × PC^0,75): com perfis mais ativos, a energia fica acima da média observada em cães que emagreceram (AAHA, 2021). O cálculo aplica a restrição sobre essa energia."}
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
                    Estimativa de perda: {(nivel.percentualPerdaSemanal * 100).toFixed(1)}% do peso inicial por semana
                  </span>
                </div>
              </Radio>
            ))}
          </Radio.Group>
        </Form.Item>

        {perdaAcimaDoRecomendadoGato && (
          <Alert
            type="warning"
            showIcon
            message={`Perda acima de ${LIMITE_PERDA_RECOMENDADA_GATO}% por semana pode ser prejudicial ao gato`}
            description="Para gatos, o recomendado é perder até 1% do peso por semana (Carciofi, 2013; Gomes, 2016). Perdas mais rápidas aumentam o risco de lipidose hepática. O nível escolhido continua permitido, a critério do veterinário."
          />
        )}
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
