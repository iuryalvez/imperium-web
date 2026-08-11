import React, { useEffect } from "react";
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

const FormEdicaoRaca = ({
  onFinish,
  onCancel,
  isSubmitting,
  initialValues,
}) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (initialValues) {
      // Converte a espécie de string ("Cao") para número (1) para preencher o formulário
      const especieComoNumero = initialValues.especie === "Cao" ? 1 : 2;
      form.setFieldsValue({
        ...initialValues,
        especie: especieComoNumero,
      });
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish} autoComplete="off">
      {/* Campo 'id' oculto, mas necessário para o backend */}
      <Form.Item name="id" hidden>
        <Input />
      </Form.Item>

      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item
            name="nome"
            label="Nome da Raça"
            rules={[{ required: true }]}
          >
            <Input />
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
              style={{ width: "100%" }}
              min={0}
              step={0.1}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMaximoReferenciaFemea" label="Peso Máximo (kg)">
            <InputNumber
              style={{ width: "100%" }}
              min={0}
              step={0.1}
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
              style={{ width: "100%" }}
              min={0}
              step={0.1}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item name="pesoMaximoReferenciaMacho" label="Peso Máximo (kg)">
            <InputNumber
              style={{ width: "100%" }}
              min={0}
              step={0.1}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="flex justify-end gap-2 mt-6">
        <Button onClick={onCancel}>Cancelar</Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormEdicaoRaca;
