import React from "react";
import { Form, InputNumber, Row, Col } from "antd";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const FormCrescimento = () => (
  <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
    <h4 className="text-sm font-semibold text-gray-700 mb-3">
      Ajuste Específico de Crescimento
    </h4>
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
            min={0}
            step={0.01}
            decimalSeparator=","
          />
        </Form.Item>
      </Col>
    </Row>
  </div>
);

export default FormCrescimento;
