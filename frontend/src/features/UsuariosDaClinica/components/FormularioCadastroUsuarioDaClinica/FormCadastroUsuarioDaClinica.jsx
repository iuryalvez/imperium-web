import React from "react";
import {
  Form,
  Input,
  Checkbox,
  Button,
  Row,
  Col,
  Divider,
  Switch,
} from "antd";

const FormCadastroUsuarioDaClinica = ({ onFinish, onCancel, isSubmitting }) => {
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
    <Form form={form} layout="vertical" onFinish={onFinish}>
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
          >
            <Input placeholder="exemplo@email.com" />
          </Form.Item>

          <Divider orientation="left" plain>
            Segurança
          </Divider>
          <Form.Item name="senha" label="Senha" rules={[{ required: true }]}>
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

      <div style={{ textAlign: "right", marginTop: 24 }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>
          Cancelar
        </Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Cadastrar
        </Button>
      </div>
    </Form>
  );
};

export default FormCadastroUsuarioDaClinica;
