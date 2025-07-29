import React from "react";
import { Form, Input, Checkbox, Row, Col, Divider } from "antd";
import { IMaskInput } from "react-imask";

// Estilo para os inputs com máscara (pode ser movido para um arquivo de estilos comum)
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

// Perfis disponíveis no contexto da clínica (sem 'ADMIN')
const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const EdicaoUsuarioDaClinica = ({ form }) => {
  return (
    <Row gutter={24}>
      {/* --- Coluna 1: Informações Pessoais e Contato --- */}
      <Col xs={24} lg={12}>
        <Divider orientation="left" plain>
          Dados Pessoais
        </Divider>
        {/* Campos ocultos necessários para a requisição */}
        <Form.Item name="id" hidden>
          <Input />
        </Form.Item>
        <Form.Item name="upcId" hidden>
          <Input />
        </Form.Item>

        <Form.Item
          label="Nome Completo"
          name="nome"
          rules={[{ required: true, message: "O nome é obrigatório" }]}
        >
          <Input placeholder="Digite o nome completo" />
        </Form.Item>
        <Form.Item
          label="E-mail"
          name="email"
          rules={[
            { required: true, message: "O e-mail é obrigatório" },
            { type: "email" },
          ]}
        >
          <Input placeholder="exemplo@email.com" />
        </Form.Item>
        <Form.Item label="CPF" name="cpf">
          <IMaskInput
            mask="000.000.000-00"
            unmask={true}
            style={inputStyle}
            placeholder="000.000.000-00"
            // Garante a atualização do Form com o valor limpo
            onAccept={(value) => form.setFieldsValue({ cpf: value })}
          />
        </Form.Item>
        <Form.Item label="Formação" name="formacao">
          <Input placeholder="Ex: Médico Veterinário" />
        </Form.Item>

        <Divider orientation="left" plain>
          Contato
        </Divider>
        <Form.Item label="Telefone Principal" name="telefone1">
          <IMaskInput
            mask="(00) 00000-0000"
            unmask={true}
            style={inputStyle}
            placeholder="(00) 99999-9999"
            onAccept={(value) => form.setFieldsValue({ telefone1: value })}
          />
        </Form.Item>
        <Form.Item label="WhatsApp" name="whatsapp">
          <IMaskInput
            mask="(00) 00000-0000"
            unmask={true}
            style={inputStyle}
            placeholder="(00) 99999-9999"
            onAccept={(value) => form.setFieldsValue({ whatsapp: value })}
          />
        </Form.Item>
      </Col>

      {/* --- Coluna 2: Acesso e Permissões --- */}
      <Col xs={24} lg={12}>
        <Divider orientation="left" plain>
          Permissões na Clínica
        </Divider>
        <Form.Item
          label="Perfis"
          name="perfis"
          rules={[{ required: true, message: "Selecione ao menos um perfil" }]}
        >
          <Checkbox.Group
            options={perfisClinica} // Usa a lista de perfis específica da clínica
            style={{ display: "flex", flexDirection: "column", gap: 8 }}
          />
        </Form.Item>
      </Col>
    </Row>
  );
};

export default EdicaoUsuarioDaClinica;
