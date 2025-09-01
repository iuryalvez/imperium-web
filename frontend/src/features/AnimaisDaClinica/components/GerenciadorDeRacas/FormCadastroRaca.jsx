import React from "react";
import {
  Form,
  Input,
  Button,
  Radio,
  InputNumber,
  Row,
  Col,
  Divider,
} from "antd";

const opcoesEspecie = [
  { label: "Cão", value: 1 },
  { label: "Gato", value: 2 },
];

const FormCadastroRaca = ({ onFinish, onCancel, isSubmitting }) => {
  return (
    <Form layout="vertical" onFinish={onFinish} autoComplete="off">
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item
            name="nome"
            label="Nome da Raça"
            rules={[{ required: true }]}
          >
            <Input placeholder="Ex: Golden Retriever" />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            name="especie"
            label="Espécie"
            rules={[{ required: true }]}
          >
            <Radio.Group
              options={opcoesEspecie}
              optionType="button"
              buttonStyle="solid"
            />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" plain>
        Peso de Referência para Fêmeas (Opcional)
      </Divider>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMinimoReferenciaFemea" label="Peso Mínimo (kg)">
            <InputNumber
              placeholder="0,00"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMaximoReferenciaFemea" label="Peso Máximo (kg)">
            <InputNumber
              placeholder="0,00"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" plain>
        Peso de Referência para Machos (Opcional)
      </Divider>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMinimoReferenciaMacho" label="Peso Mínimo (kg)">
            <InputNumber
              placeholder="0,00"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMaximoReferenciaMacho" label="Peso Máximo (kg)">
            <InputNumber
              placeholder="0,00"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" plain>
        Referência (Opcional)
      </Divider>

      <Form.Item name="referencia" label="Referência">
        <Input placeholder="Ex: Mark Morris Institute" />
      </Form.Item>

      <div className="flex justify-end gap-2 mt-6">
        <Button onClick={onCancel}>Cancelar</Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Cadastrar
        </Button>
      </div>
    </Form>
  );
};

export default FormCadastroRaca;
