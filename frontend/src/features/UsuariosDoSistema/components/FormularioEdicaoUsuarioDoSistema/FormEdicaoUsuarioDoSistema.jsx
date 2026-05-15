import React from "react";
import {
  Form,
  Input,
  Checkbox,
  Button,
  Row,
  Col,
  Divider,
  Tag,
  Switch,
} from "antd";
import { CpfMaskInput, TelefoneMaskInput } from "../../../../utils/masks";
import { validarCPF } from "../../../../utils/validators";

const FormEdicaoUsuarioDoSistema = ({
  onFinish,
  onCancel,
  isSubmitting,
  initialValues,
}) => {
  const [form] = Form.useForm();

  const isAdmin = initialValues?.isAdmin || false;

  const perfisOptions = [
    { label: "Gestor", value: "GESTOR" },
    { label: "Veterinário", value: "VETERINARIO" },
    // { label: "Zootecnista", value: "ZOOTECNISTA" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Técnico", value: "TECNICO" },
  ];

  return (
    // Usamos initialValues aqui para que o Form controle o preenchimento inicial
    <Form
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
      <Form.Item name="isAdmin" hidden>
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
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="email"
            label="E-mail"
            rules={[{ required: true, type: "email" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="CPF"
            name="cpf"
            rules={[
              { required: true, message: "O CPF é obrigatório" },
              () => ({
                validator(_, value) {
                  if (!value || validarCPF(value)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("CPF inválido"));
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

          {isAdmin ? (
            // --- Lógica para Administradores ---
            <>
              <Form.Item label="Perfil de acesso">
                <Tag color="blue">Administrador do sistema</Tag>
              </Form.Item>
            </>
          ) : (
            // --- Lógica para Usuários de Clínica ---
            <>
              <Form.Item
                label="Perfis na Clínica"
                name="perfis"
                rules={[{ required: true }]}
              >
                <Checkbox.Group
                  options={perfisOptions}
                  style={{ display: "flex", flexDirection: "column", gap: 8 }}
                />
              </Form.Item>
              <Form.Item label="Clínica">
                <Input
                  value={initialValues?.nomeClinica || "Não informado"}
                  disabled
                />
              </Form.Item>
            </>
          )}

          <Form.Item
            label="Status de acesso"
            name="isAtivo"
            valuePropName="checked"
          >
            <Switch checkedChildren="ATIVO" unCheckedChildren="INATIVO" />
          </Form.Item>

          <Divider orientation="left" plain>
            Alterar Senha (Opcional)
          </Divider>
          <Form.Item name="senha" label="Nova Senha">
            <Input.Password placeholder="Deixe em branco para não alterar" />
          </Form.Item>
          <Form.Item
            name="confirmarSenha"
            dependencies={["senha"]}
            rules={[
              ({ getFieldValue }) => ({
                required: !!getFieldValue("senha"),
                message: "Confirme a nova senha",
              }),
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("senha") === value)
                    return Promise.resolve();
                  return Promise.reject(new Error("As senhas não coincidem!"));
                },
              }),
            ]}
          >
            <Input.Password placeholder="Repita a nova senha" />
          </Form.Item>
        </Col>
      </Row>

      <div style={{ textAlign: "right", marginTop: 24 }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>
          Cancelar
        </Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormEdicaoUsuarioDoSistema;
