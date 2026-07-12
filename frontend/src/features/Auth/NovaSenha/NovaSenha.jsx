import React from "react";
import { Typography, Form, Button, Input } from "antd";
import { LockOutlined } from "@ant-design/icons";
import { useNovaSenha } from "./useNovaSenha";
import { Link } from "react-router-dom";

const { Title, Paragraph } = Typography;

const NovaSenha = () => {
  const { loading, isValidLink, email, onFinish } = useNovaSenha();

  if (!isValidLink) {
    return (
      <div className="min-h-[calc(100vh-64px)] flex flex-col items-start justify-center py-12 bg-verde-fraco w-full">
        <Title level={3} className="text-red-500 mb-4">
          Link Inválido ou Ausente
        </Title>
        <Link to="/recuperar-senha">
          <Button type="primary" size="large">
            Solicitar novo link
          </Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-64px)] flex items-start justify-center py-12 bg-verde-fraco w-full">
      <div className="flex flex-col items-center bg-white rounded-3xl shadow-lg p-10 w-full max-w-[450px]">
        <Title level={2} className="text-verde-medio mb-2">
          Nova Senha
        </Title>
        <Paragraph className="text-center text-gray-500 mb-6">
          Defina uma nova senha para a conta: <br />
          <strong>{email}</strong>
        </Paragraph>

        <Form
          name="nova_senha"
          onFinish={onFinish}
          layout="vertical"
          className="w-full"
          requiredMark={false}
        >
          <Form.Item
            name="senha"
            label="Nova Senha"
            hasFeedback
            rules={[
              {
                required: true,
                message: "A senha é obrigatória.",
              },
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
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="******"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="confirmarSenha"
            label="Confirmar Senha"
            dependencies={["senha"]}
            hasFeedback
            rules={[
              {
                required: true,
                message: "A confirmação da senha é obrigatória.",
              },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("senha") === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("As senhas não coincidem."));
                },
              }),
            ]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="******"
              size="large"
            />
          </Form.Item>

          <Form.Item className="mt-8 mb-0">
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              block
              size="large"
            >
              Redefinir Senha
            </Button>
          </Form.Item>
        </Form>
      </div>
    </div>
  );
};

export default NovaSenha;
