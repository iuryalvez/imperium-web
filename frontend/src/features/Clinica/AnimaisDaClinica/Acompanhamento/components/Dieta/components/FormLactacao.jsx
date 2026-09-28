import React from "react";
import {
  Form,
  Row,
  Col,
  Select,
  InputNumber,
  Alert,
  Collapse,
  List,
} from "antd";
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";
import FormulaPreview from "./FormulaPreview";

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

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Lactação
      </h4>

      {isCao && (
        <Alert
          message="Recomendação para Cadelas (Customização Obrigatória)"
          description="Para cadelas em lactação, a literatura recomenda extrapolar a energia de manutenção. Portanto, é obrigatório preencher manualmente o K e o P."
          type="info"
          showIcon
          className="mb-4"
        />
      )}

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

      {isGato && (
        <>
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
        </>
      )}

      {isCao && (
        <Collapse
          size="small"
          className="mb-4 bg-white"
          items={[
            {
              key: "1",
              label: "Perfis Metabólicos de Manutenção Conhecidos",
              children: (
                <div className="max-h-60 overflow-y-auto">
                  <List
                    size="small"
                    dataSource={perfisParaExibir.filter(
                      (p) =>
                        p.tiposDieta?.includes(1) &&
                        p.label.toLowerCase().includes("cão"),
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
