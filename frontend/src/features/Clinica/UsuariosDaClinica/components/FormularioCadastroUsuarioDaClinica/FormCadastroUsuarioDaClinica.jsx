import React from "react";
import { Form, Input, Checkbox, Row, Col, Divider, Switch } from "antd";

const FormCadastroUsuarioDaClinica = ({ onFinish }) => {
  const [form] = Form.useForm();

  // Opções de perfis para clínica
  const perfisOptions = [
    { label: "Gestor", value: "GESTOR" },
    { label: "Veterinário", value: "VETERINARIO" },
    // { label: "Zootecnista", value: "ZOOTECNISTA" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Técnico", value: "TECNICO" },
  ];

  // Handler para corrigir a seleção de perfis
  const handlePerfisChange = (selecaoAtual) => {
    let selecaoCorrigida = [...selecaoAtual];

    // Atualiza o valor no formulário apenas se a lógica alterou a seleção do usuário.
    if (JSON.stringify(selecaoCorrigida) !== JSON.stringify(selecaoAtual)) {
      form.setFieldsValue({ perfis: selecaoCorrigida });
    }
  };

  return (
    <Form id="form-cadastro-usuario-clinica" form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        {/* --- Coluna 1: Informações Pessoais --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Dados Pessoais
          </Divider>
          <Form.Item
            name="nome"
            label="Nome Completo"
            rules={[{ required: true }]}
          >
            <Input placeholder="Digite o nome completo" />
          </Form.Item>
          <Form.Item
            name="email"
            label="E-mail"
            rules={[{ required: true, type: "email" }]}
            extra="Se o e-mail já tiver cadastro no sistema, o usuário só será vinculado à clínica com os perfis escolhidos; os outros dados e a senha dele não mudam."
          >
            <Input placeholder="exemplo@email.com" />
          </Form.Item>

          <Divider orientation="left" plain>
            Segurança
          </Divider>
          <Form.Item
            name="senha"
            label="Senha"
            hasFeedback
            rules={[
              { required: true, message: "A senha é obrigatória." },
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
            <Input.Password placeholder="Crie uma senha forte" />
          </Form.Item>
          <Form.Item
            name="confirmarSenha"
            label="Confirmar Senha"
            dependencies={["senha"]}
            rules={[
              { required: true },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("senha") === value)
                    return Promise.resolve();
                  return Promise.reject(new Error("As senhas não coincidem!"));
                },
              }),
            ]}
          >
            <Input.Password placeholder="Digite a senha novamente" />
          </Form.Item>
        </Col>

        {/* --- Coluna 2: Acesso e Permissões --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Permissões e Acesso
          </Divider>

          <Form.Item name="perfis" label="Perfis" rules={[{ required: true }]}>
            <Checkbox.Group
              options={perfisOptions}
              onChange={handlePerfisChange}
              style={{ display: "flex", flexDirection: "column", gap: 8 }}
            />
          </Form.Item>

          <Form.Item
            label="Status"
            name="isAtivo"
            valuePropName="checked"
            initialValue={true}
          >
            <Switch checkedChildren="ATIVO" unCheckedChildren="INATIVO" />
          </Form.Item>
        </Col>
      </Row>
    </Form>
  );
};

export default FormCadastroUsuarioDaClinica;
