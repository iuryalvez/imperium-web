import React from "react";
import {
  Form,
  Row,
  Col,
  Select,
  InputNumber,
  Alert,
} from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";
import { useEffect } from "react";

export const MAX_FILHOTES_FONTE = 8;

const FormLactacao = ({ form, especie, perfis, loading }) => {
  const perfisParaExibir = perfis || [];
  const multiplicadorCustom = Form.useWatch(
    "multiplicadorEnergetico",
    form,
  );
  const potencia = Form.useWatch("potencia", form);
  const pesoAtualWatch = Form.useWatch("pesoAtual", form);
  const perfilMetabolicoId = Form.useWatch("perfilMetabolico", form);
  const semanaLactacaoWatch = Form.useWatch("semanaLactacao", form);
  const numeroFilhotesWatch = Form.useWatch("numeroFilhotes", form);

  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";
  const isCao = especie === 1 || String(especie).toLowerCase() === "cao";

  const perfilSelecionado = perfisParaExibir.find(
    (p) => p.value === perfilMetabolicoId,
  );
  const perfilMetabolicoLabel = perfilSelecionado?.label;
  // As tabelas do NRC/FEDIAF vão até 8 filhotes; acima disso o cálculo é extrapolado.
  const foraDaFaixaDaFonte = numeroFilhotesWatch > MAX_FILHOTES_FONTE;

  useEffect(() => {
    if (!perfilMetabolicoId && (perfis?.length > 0)) {
      if (isCao) form.setFieldValue("perfilMetabolico", 11);
      if (isGato) form.setFieldValue("perfilMetabolico", 10);
    }
  }, [isCao, isGato, perfilMetabolicoId, perfis, form]);

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Lactação
      </h4>

      <Alert
        message={
          isGato
            ? "Recomendação: 100 × PC^0,67 + energia da lactação"
            : "Recomendação: 145 × PC^0,75 + energia da lactação"
        }
        description={
          isGato
            ? "O perfil Gata em Lactação preenche K = 100 e P = 0,67. Os valores podem ser personalizados conforme a avaliação clínica."
            : "O perfil Cadela em Lactação preenche K = 145 e P = 0,75. Os valores podem ser personalizados conforme a avaliação clínica."
        }
        type="info"
        showIcon
        className="mb-4"
      />

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            name="numeroFilhotes"
            label={
              <LabelComInfo
                label="Número de Filhotes"
                tituloPopover="Quantidade de Filhotes"
                conteudoPopover="Impacta diretamente na constante de adição de energia (para gatos) e na quantidade de energia adicionada por filhote (para cães)."
              />
            }
            rules={[{ required: true, message: "Obrigatório" }]}
          >
            <InputNumber
              placeholder="Ex: 4"
              style={{ width: "100%" }}
              min={1}
              max={20}
              step={1}
            />
          </Form.Item>
        </Col>

        <Col xs={24} md={12}>
          <Form.Item
            name="semanaLactacao"
            label={
              <LabelComInfo
                label="Semana de Lactação"
                tituloPopover="Semana de Lactação"
                conteudoPopover={
                  isGato
                    ? "Semanas de 1 a 7 (Fator L)"
                    : "Semanas de 1 a 4 (Fator L)"
                }
              />
            }
            rules={[{ required: true, message: "Obrigatório" }]}
          >
            <InputNumber
              placeholder="Ex: 2"
              style={{ width: "100%" }}
              min={1}
              max={isGato ? 7 : 4}
              step={1}
            />
          </Form.Item>
        </Col>
      </Row>

      {foraDaFaixaDaFonte && (
        <Alert
          type="warning"
          showIcon
          className="mb-4"
          message={`Mais de ${MAX_FILHOTES_FONTE} filhotes: fora da faixa da fonte`}
          description="As tabelas do NRC (2006) e da FEDIAF (2025) vão até 8 filhotes. O sistema calcula com a mesma equação, mas o valor é uma estimativa e deve ser acompanhado de perto."
        />
      )}

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico"
                tituloPopover="Perfil Metabólico"
                conteudoPopover="Permite selecionar um perfil pré-cadastrado."
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

      <FormulaPreview
        tipoDieta={3} // Lactação
        isGato={isGato}
        isCustomizado={false}
        multiplicadorCustom={multiplicadorCustom}
        potencia={potencia}
        pesoAtual={pesoAtualWatch}
        perfilMetabolicoLabel={perfilMetabolicoLabel}
        numeroFilhotes={numeroFilhotesWatch}
        semanaLactacao={semanaLactacaoWatch}
      />

      <Row gutter={16}>
        <Col xs={24} sm={12}>
            <Form.Item
              name="multiplicadorEnergetico"
              label="Multiplicador (K)"
              rules={[{ required: true, message: "Insira o K" }]}
            >
              <InputNumber
                style={{ width: "100%" }}
                min={1}
                max={300}
                step={0.1}
                decimalSeparator=","
                placeholder="Ex: 145,0"
              />
            </Form.Item>
          </Col>
          <Col xs={24} sm={12}>
            <Form.Item
              name="potencia"
              label="Potência (P)"
              rules={[{ required: true, message: "Insira o P" }]}
            >
              <InputNumber
                style={{ width: "100%" }}
                min={0.1}
                max={1.0}
                step={0.01}
                decimalSeparator=","
                placeholder="Ex: 0,75"
              />
            </Form.Item>
          </Col>
      </Row>
    </div>
  );
};

export default FormLactacao;
