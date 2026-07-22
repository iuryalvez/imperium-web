import React from "react";
import { Form, Input, Checkbox, Row, Col, Divider, Switch } from "antd";
import { CpfMaskInput, TelefoneMaskInput } from "../../../../../utils/masks";
import { validarCPF } from "../../../../../utils/validators";

const FormEdicaoUsuarioDaClinica = ({
  onFinish,
  initialValues,
}) => {
  const [form] = Form.useForm();

  // Observa o campo "senha" em tempo real para exibir a confirmação dinamicamente
  const senhaDigitada = Form.useWatch("senha", form);

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
          <Form.Item
            name="nome"
            label="Nome Completo"
            rules={[{ required: true, message: "O nome é obrigatório." }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="email"
            label="E-mail"
            rules={[
              {
                required: true,
                type: "email",
                message: "Insira um e-mail válido.",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="CPF"
            name="cpf"
            rules={[
              { required: true, message: "O CPF é obrigatório." },
              () => ({
                validator(_, value) {
                  if (!value || validarCPF(value)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("CPF inválido."));
                },
              }),
            ]}
          >
            <CpfMaskInput />
          </Form.Item>
          <Form.Item label="Formação" name="formacao">
            <Input placeholder="Ex: Médico Veterinário" />
          </Form.Item>

          <Divider orientation="left" plain>
            Contato
          </Divider>
          <Form.Item label="Telefone" name="telefone">
            <TelefoneMaskInput />
          </Form.Item>

          <Form.Item label="WhatsApp" name="whatsapp">
            <TelefoneMaskInput />
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

          <Divider orientation="left" plain>
            Alterar Senha (Opcional)
          </Divider>

          <Form.Item
            name="senha"
            label="Nova Senha"
            hasFeedback
            // extra="Deixe em branco caso não queira alterar a senha atual."
            rules={[
              // Regras ignoradas pelo Ant Design se o campo estiver vazio
              { min: 8, message: "A senha deve ter no mínimo 8 caracteres." },
              {
                pattern: /[A-Z]/,
                message: "Deve conter pelo menos uma letra maiúscula.",
              },
              {
                pattern: /[a-z]/,
                message: "Deve conter pelo menos uma letra minúscula.",
              },
              {
                pattern: /[0-9]/,
                message: "Deve conter pelo menos um número.",
              },
              {
                pattern: /[^A-Za-z0-9]/,
                message: "Deve conter pelo menos um caractere especial.",
              },
            ]}
          >
            <Input.Password placeholder="Deixe em branco para não alterar" />
          </Form.Item>

          {/* Renderiza o confirmar senha APENAS se algo for digitado no campo senha */}
          {senhaDigitada && (
            <Form.Item
              name="confirmarSenha"
              label="Confirmar Nova Senha"
              dependencies={["senha"]}
              hasFeedback
              rules={[
                {
                  required: true,
                  message: "A confirmação da nova senha é obrigatória.",
                },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue("senha") === value) {
                      return Promise.resolve();
                    }
                    return Promise.reject(
                      new Error("As senhas não coincidem."),
                    );
                  },
                }),
              ]}
            >
              <Input.Password placeholder="Repita a nova senha" />
            </Form.Item>
          )}
        </Col>
      </Row>
    </Form>
  );
};

export default FormEdicaoUsuarioDaClinica;
