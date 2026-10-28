import React from "react";
import { Form, Input, Checkbox, Row, Col, Divider, Switch, Alert } from "antd";
import { CpfMaskInput, TelefoneMaskInput } from "../../../../../utils/masks";

const FormEdicaoUsuarioDaClinica = ({
  onFinish,
  initialValues,
}) => {
  const [form] = Form.useForm();

  const perfisOptions = [
    { label: "Gestor", value: "GESTOR" },
    { label: "Veterinário", value: "VETERINARIO" },
    // { label: "Zootecnista", value: "ZOOTECNISTA" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Técnico", value: "TECNICO" },
  ];

  return (
    <Form
      id="form-edicao-usuario-clinica"
      form={form}
      layout="vertical"
      onFinish={onFinish}
      initialValues={initialValues}
    >
      {/* IDs necessários para o backend, mas ocultos para o usuário */}
      <Form.Item name="id" hidden>
        <Input />
      </Form.Item>
      <Form.Item name="upcId" hidden>
        <Input />
      </Form.Item>

      <Row gutter={24}>
        {/* --- Coluna 1: Informações Pessoais --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Dados Pessoais
          </Divider>
          <Alert
            type="info"
            showIcon
            className="mb-4"
            message="Os dados pessoais e a senha só podem ser alterados pelo próprio usuário (Meu Perfil) ou pelo administrador do sistema."
          />
          <Form.Item
            name="nome"
            label="Nome Completo"
          >
            <Input disabled />
          </Form.Item>
          <Form.Item
            name="email"
            label="E-mail"
          >
            <Input disabled />
          </Form.Item>
          <Form.Item
            label="CPF"
            name="cpf"
          >
            <CpfMaskInput disabled />
          </Form.Item>
          <Form.Item label="Formação" name="formacao">
            <Input disabled />
          </Form.Item>

          <Divider orientation="left" plain>
            Contato
          </Divider>
          <Form.Item label="Telefone" name="telefone">
            <TelefoneMaskInput disabled />
          </Form.Item>

          <Form.Item label="WhatsApp" name="whatsapp">
            <TelefoneMaskInput disabled />
          </Form.Item>
        </Col>

        {/* --- Coluna 2: Acesso e Permissões --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Permissões e Acesso
          </Divider>

          <Form.Item
            label="Perfis na Clínica"
            name="perfis"
            rules={[
              { required: true, message: "Selecione pelo menos um perfil." },
            ]}
          >
            <Checkbox.Group
              options={perfisOptions}
              style={{ display: "flex", flexDirection: "column", gap: 8 }}
            />
          </Form.Item>

          <Form.Item
            label="Status de acesso"
            name="isAtivoNaClinica"
            valuePropName="checked"
          >
            <Switch checkedChildren="ATIVO" unCheckedChildren="INATIVO" />
          </Form.Item>

        </Col>
      </Row>
    </Form>
  );
};

export default FormEdicaoUsuarioDaClinica;
