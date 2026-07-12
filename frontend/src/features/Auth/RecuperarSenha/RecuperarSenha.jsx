import React from "react";
import { Typography, Form, Button, Input } from "antd";
import { UserOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import { useRecuperarSenha } from "./useRecuperarSenha";
import { Link } from "react-router-dom";

const { Title, Paragraph } = Typography;

const RecuperarSenha = () => {
  const { loading, onFinish } = useRecuperarSenha();

  return (
    <div className="min-h-[calc(100vh-64px)] flex items-start justify-center py-12 bg-verde-fraco w-full">
      <div className="flex flex-col items-center bg-white rounded-3xl shadow-lg p-10 w-full max-w-[450px]">
        <Title level={2} className="text-verde-medio mb-4">
          Recuperar Senha
        </Title>
        <Paragraph className="text-center text-gray-500 mb-8">
          Digite o e-mail associado à sua conta. Enviaremos um link para você redefinir sua senha.
        </Paragraph>

        <Form
          name="recuperar"
          onFinish={onFinish}
          layout="vertical"
          className="w-full"
          requiredMark={false}
        >
          <Form.Item
            name="email"
            label="E-mail"
            rules={[{ required: true, type: "email", message: "Insira um e-mail válido." }]}
          >
            <Input prefix={<UserOutlined />} placeholder="exemplo@email.com" size="large" />
          </Form.Item>

          <Form.Item className="mt-6">
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              Enviar Link
            </Button>
          </Form.Item>
        </Form>

        <div className="mt-4 w-full text-center">
          <Link to="/login" className="flex items-center justify-center text-verde-medio hover:underline">
            <ArrowLeftOutlined className="mr-2" /> Voltar para o Login
          </Link>
        </div>
      </div>
    </div>
  );
};

export default RecuperarSenha;