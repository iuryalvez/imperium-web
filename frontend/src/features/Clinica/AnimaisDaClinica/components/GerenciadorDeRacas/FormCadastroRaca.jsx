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
  Select,
} from "antd";

const opcoesEspecie = [
  { label: "Cão", value: 1 },
  { label: "Gato", value: 2 },
];

const opcoesCategoriaMetabolica = [
  { label: "Padrão", value: 0 },
  { label: "Terrier", value: 1 },
  { label: "Dogue Alemão", value: 2 },
  { label: "Terra Nova", value: 3 },
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

      <Row gutter={24}>
        <Col xs={24}>
          <Form.Item
            name="categoriaMetabolica"
            label="Categoria Metabólica (Apenas para cães)"
            tooltip="Algumas raças caninas possuem necessidades energéticas diferenciadas segundo o NRC."
            initialValue={0}
          >
            <Select
              options={opcoesCategoriaMetabolica}
              placeholder="Selecione a categoria metabólica"
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
              placeholder="Ex: 9,87"
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
              placeholder="Ex: 9,87"
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
              placeholder="Ex: 9,87"
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
              placeholder="Ex: 9,87"
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
