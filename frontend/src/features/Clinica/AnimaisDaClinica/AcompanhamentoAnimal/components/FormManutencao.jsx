// .../FormularioNovaDieta/FormManutencao.jsx
import React from "react";
import { Form, InputNumber, Row, Col, Collapse, Typography } from "antd";

const { Text } = Typography;

const FormManutencao = () => (
  <>
    <Row gutter={16}>
      <Col span={8}>
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
      <Col span={8}>
        <Form.Item name="pesoObjetivo" label="Peso Objetivo (kg)">
          <InputNumber
            placeholder="Ex: 11,0"
            style={{ width: "100%" }}
            min={0}
            step={0.01}
            decimalSeparator=","
          />
        </Form.Item>
      </Col>
    </Row>
    {/* <Collapse size="small">
      <Collapse.Panel header="Opções Avançadas de Cálculo" key="1">
        <Text type="secondary">
          Use estes campos apenas para substituir o cálculo padrão do sistema.
        </Text>
        <Row gutter={16} style={{ marginTop: 16 }}>
          <Col span={12}>
            <Form.Item
              name="multiplicadorEnergeticoCustom"
              label="Multiplicador Energético (k)"
            >
              <InputNumber style={{ width: "100%" }} placeholder="Ex: 130" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="potenciaCustom" label="Potência (expoente)">
              <InputNumber style={{ width: "100%" }} placeholder="Ex: 0.75" />
            </Form.Item>
          </Col>
        </Row>
      </Collapse.Panel>
    </Collapse> */}
  </>
);

export default FormManutencao;
