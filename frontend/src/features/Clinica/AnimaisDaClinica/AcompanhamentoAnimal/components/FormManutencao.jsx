import React from "react";
import { Form, InputNumber, Row, Col } from "antd";

const FormManutencao = () => (
  <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
    <h4 className="text-sm font-semibold text-gray-700 mb-3">
      Ajuste Específico de Manutenção
    </h4>
    <Row gutter={16}>
      <Col span={24}>
        <Form.Item
          name="pesoObjetivo"
          label="Peso Alvo do Ciclo (kg)"
          tooltip="A meta de peso apenas para esta prescrição específica (ex: emagrecimento gradual ou ganho de massa). Se deixado em branco, o sistema usará o Peso Referência."
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
  </div>
);

export default FormManutencao;
