import React from "react";
import {
  Form,
  Input,
  Checkbox,
  Typography,
  Tag,
  Row,
  Col,
  Divider,
} from "antd";
import { IMaskInput } from "react-imask";

// Estilo para os inputs com máscara
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

const perfisSistema = [
  // "ADMIN" não deve ser selecionável
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const EdicaoUsuarioDoSistema = ({ form, valoresIniciais }) => {
  const isAdmin = valoresIniciais?.isAdmin || false;

  return (
    <Row gutter={24}>
      {/* --- Coluna 1: Informações Pessoais --- */}
      <Col xs={24} lg={12}>
        <Divider orientation="left" plain>
          Dados Pessoais
        </Divider>
        {/* Campos ocultos para garantir que os IDs sejam enviados na submissão */}
        <Form.Item name="id" hidden>
          <Input />
        </Form.Item>
        <Form.Item name="upcId" hidden>
          <Input />
        </Form.Item>
        <Form.Item name="isAdmin" hidden>
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
          Permissões e Acesso
        </Divider>

        {isAdmin ? (
          // --- Lógica para Administradores ---
          // aqui deve ser div para o form.item ser identificado como um item do form pai lá da minha formEdicaoEntidade
          <div>
            <Typography.Text strong>Perfil de Acesso: </Typography.Text>
            <Tag color="blue">
              Admin
            </Tag>

            <Form.Item
              label="Status do Usuário"
              name="isAtivo"
              valuePropName="checked"
              style={{ marginTop: "24px" }}
            >
              <Checkbox>Usuário Ativo</Checkbox>
            </Form.Item>
          </div>
        ) : (
          // --- Lógica para Usuários de Clínica ---
          <>
            <Form.Item
              label="Perfis na Clínica"
              name="perfis"
              rules={[
                { required: true, message: "Selecione ao menos um perfil" },
              ]}
            >
              <Checkbox.Group
                options={perfisSistema}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
              />
            </Form.Item>
            <Form.Item label="Clínica">
              <Input
                value={valoresIniciais?.nomeClinica || "Não informado"}
                disabled
              />
            </Form.Item>
          </>
        )}

        <Divider orientation="left" plain>
          Alterar Senha (Opcional)
        </Divider>
        <Form.Item label="Nova Senha" name="senha">
          <Input.Password placeholder="Deixe em branco para não alterar" />
        </Form.Item>
        <Form.Item
          label="Confirmar Nova Senha"
          name="confirmarSenha"
          dependencies={["senha"]}
          rules={[
            // A confirmação só é obrigatória se o campo de senha for preenchido
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
  );
};

export default EdicaoUsuarioDoSistema;
