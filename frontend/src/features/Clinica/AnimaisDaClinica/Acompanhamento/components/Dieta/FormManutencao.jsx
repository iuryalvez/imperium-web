import React from "react";
import { Form, Row, Col, Select } from "antd";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const FormManutencao = ({ form, especie, perfis, loading }) => {
  const perfisParaExibir = perfis || [];
  const isCustomizado = Form.useWatch("isCustomizado", form);

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Manutenção
      </h4>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico (Fórmula NRC)"
                tituloPopover="Perfil Metabólico"
                conteudoPopover="Se deixado em branco, o sistema tentará calcular a fórmula automaticamente com base nos dados cadastrados."
              />
            }
          >
            <Select
              allowClear
              loading={loading}
              disabled={isCustomizado}
              options={perfisParaExibir}
              placeholder={
                isCustomizado
                  ? "Desabilitado pela customização..."
                  : "Selecione um perfil para uma fórmula específica..."
              }
            />
          </Form.Item>
        </Col>
      </Row>
    </div>
  );
};

export default FormManutencao;
