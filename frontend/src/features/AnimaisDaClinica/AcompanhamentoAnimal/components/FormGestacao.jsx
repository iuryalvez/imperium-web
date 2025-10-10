import React from "react";
import { Form, InputNumber, Row, Col } from "antd";

const FormGestacao = () => (
  <Row gutter={16}>
    <Col span={12}>
      <Form.Item
        name="pesoAtual"
        label="Peso Atual (kg)"
        rules={[{ required: true }]}
      >
        <InputNumber
          placeholder="Ex: 12,5"
          style={{ width: "100%" }}
          min={0}
          step={0.01}
          decimalSeparator=","
        />
      </Form.Item>
    </Col>
    <Col span={12}>
      <Form.Item name="pesoObjetivo" label="Peso Ideal Estimado (kg)">
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
);

export default FormGestacao;
