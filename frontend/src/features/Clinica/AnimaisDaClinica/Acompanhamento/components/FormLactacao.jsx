import React from 'react';
import { Form, InputNumber, Row, Col } from 'antd';

const FormLactacao = () => (
  <Row gutter={16}>
    <Col span={12}>
      <Form.Item name="numeroFilhotes" label="Número de Filhotes" rules={[{ required: true }]}>
        <InputNumber style={{ width: '100%' }} min={1} placeholder="Quantos filhotes?" />
      </Form.Item>
    </Col>
    <Col span={12}>
      <Form.Item name="semanaLactacao" label="Semana de Lactação" rules={[{ required: true }]}>
        <InputNumber style={{ width: '100%' }} min={1} placeholder="Qual semana?" />
      </Form.Item>
    </Col>
  </Row>
);

export default FormLactacao;