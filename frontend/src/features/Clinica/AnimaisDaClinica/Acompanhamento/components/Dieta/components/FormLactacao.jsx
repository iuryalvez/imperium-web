import React from 'react';
import { Form, InputNumber, Row, Col } from 'antd';
import LabelComInfo from "../../../../../../../components/atoms/LabelComInfo";

const FormLactacao = () => (
  <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
    <h4 className="text-sm font-semibold text-gray-700 mb-3">
      Ajuste Específico de Lactação
    </h4>
    <Row gutter={16}>
      <Col span={24}>
        <Form.Item
          name="pesoIdeal"
          label={
            <LabelComInfo
              label="Peso Ideal / Referência (kg)"
              tituloPopover="Peso Referência / Ideal"
              conteudoPopover="Se preenchido, será a base para os cálculos energéticos de lactação."
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
    </Row>
    <Row gutter={16}>
      <Col span={12}>
        <Form.Item name="numeroFilhotes" label="Número de Filhotes" rules={[{ required: true }]}>
          <InputNumber style={{ width: '100%' }} min={1} max={30} placeholder="Quantos filhotes?" />
        </Form.Item>
      </Col>
      <Col span={12}>
        <Form.Item name="semanaLactacao" label="Semana de Lactação" rules={[{ required: true }]}>
          <InputNumber style={{ width: '100%' }} min={1} max={20} placeholder="Qual semana?" />
        </Form.Item>
      </Col>
    </Row>
  </div>
);

export default FormLactacao;