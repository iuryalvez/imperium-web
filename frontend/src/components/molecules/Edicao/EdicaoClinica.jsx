import React from "react";
import { Form, Input, Switch, Row, Col, Divider } from "antd";
import { IMaskInput } from "react-imask";

const inputStyle = {
  width: "100%",
  padding: "4px 11px",
  border: "1px solid #d9d9d9",
  borderRadius: "6px",
  fontSize: "14px",
  lineHeight: "1.5715",
  transition: "all 0.2s",
  backgroundColor: "#fff",
};

const EdicaoClinica = ({ form }) => {
  return (
    <Row gutter={24}>
      <Form.Item name="id" hidden>
        <Input />
      </Form.Item>

      {/* --- Coluna 1: Identificação e Contato --- */}
      <Col xs={24} lg={12}>
        <Divider orientation="left" plain>
          Identificação
        </Divider>
        <Form.Item
          label="Nome da Clínica"
          name="nomeClinica"
          rules={[{ required: true }]}
        >
          <Input placeholder="Nome oficial da clínica" />
        </Form.Item>
        <Form.Item label="Nome Fantasia" name="nomeFantasia">
          <Input placeholder="Nome popular da clínica" />
        </Form.Item>
        <Form.Item label="CNPJ" name="cnpj">
          <IMaskInput
            mask="00.000.000/0000-00"
            unmask={true}
            style={inputStyle}
            onAccept={(v) => form.setFieldsValue({ cnpj: v })}
          />
        </Form.Item>

        <Divider orientation="left" plain>
          Contato
        </Divider>
        <Form.Item label="E-mail" name="email" rules={[{ type: "email" }]}>
          <Input placeholder="contato@clinicavet.com" />
        </Form.Item>
        <Form.Item label="Telefone Principal" name="Telefone">
          <IMaskInput
            mask="(00) 00000-0000"
            unmask={true}
            style={inputStyle}
            onAccept={(v) => form.setFieldsValue({ telefone: v })}
          />
        </Form.Item>
        <Form.Item label="WhatsApp" name="whatsapp">
          <IMaskInput
            mask="(00) 00000-0000"
            unmask={true}
            style={inputStyle}
            onAccept={(v) => form.setFieldsValue({ whatsapp: v })}
          />
        </Form.Item>
        <Form.Item label="Instagram" name="instagram">
          <Input placeholder="@seuinstagram" prefix="@" />
        </Form.Item>
      </Col>

      {/* --- Coluna 2: Endereço e Status --- */}
      <Col xs={24} lg={12}>
        <Divider orientation="left" plain>
          Endereço
        </Divider>
        <Form.Item label="CEP" name="cep">
          <IMaskInput
            mask="00000-000"
            unmask={true}
            style={inputStyle}
            onAccept={(v) => form.setFieldsValue({ cep: v })}
          />
        </Form.Item>
        <Form.Item label="Endereço (Rua, Av.)" name="endereco">
          <Input />
        </Form.Item>
        <Form.Item label="Número" name="endNumero">
          <Input />
        </Form.Item>
        <Form.Item label="Cidade" name="endCidade">
          <Input />
        </Form.Item>
        <Form.Item label="Estado" name="endEstado">
          <Input />
        </Form.Item>

        <Divider orientation="left" plain>
          Status
        </Divider>
        <Form.Item
          label="Status da Clínica"
          name="isAtiva"
          valuePropName="checked"
        >
          <Switch checkedChildren="ATIVA" unCheckedChildren="INATIVA" />
        </Form.Item>
      </Col>
    </Row>
  );
};

export default EdicaoClinica;
