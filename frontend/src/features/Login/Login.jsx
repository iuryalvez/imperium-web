import React from "react";
import {
  Row,
  Col,
  Typography,
  Space,
  Form,
  Button,
  Input,
  Checkbox,
} from "antd";
import { useAuth } from "../../services/AuthContext";
import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";
import { UserOutlined, LockOutlined } from "@ant-design/icons";

import "./Login.css";

const { Title, Paragraph } = Typography;

const Login = () => {
  const { login, isLoading } = useAuth();

  const handleLogin = async ({ email, senha }) => {
    login({ email, senha }); // chama a função login do contexto
  };

  return (
    <Row className="login-page-row">
      {/* Lado esquerdo - informações */}
      <Col span={14} className="login-info-col">
        <Title level={1}>Bem-vindo ao Software IMPERIUM!</Title>
        <Paragraph>
          Este software tem como objetivo auxiliar no controle nutricional de
          cães e gatos, utilizando técnicas de{" "}
          <strong>inteligência artificial</strong>, em especial o{" "}
          <strong>aprendizado de máquina</strong>, para padronizar a avaliação
          do Escore de Condição Corporal (ECC) dos animais. Com isso, o sistema
          contribui diretamente para a promoção da saúde e do bem-estar dos
          pets.
        </Paragraph>

        <Space size="large" className="login-logos-container">
          <img src={logoUfg} alt="Logo UFG" />
          <img src={logoEvz} alt="Logo EVZ" />
          <img src={logoInf} alt="Logo INF" />
        </Space>
      </Col>

      {/* Lado direito - formulário */}
      <Col span={10} className="login-form-col">
        <div className="login-form-container">
          <Title level={2} className="login-title">
            LOGIN
          </Title>

          <Form
            name="login"
            onFinish={handleLogin}
            layout="vertical"
            style={{ width: "100%" }}
            requiredMark={false}
          >
            <Form.Item
              name="email"
              label="E-mail"
              rules={[{ required: true, type: "email" }]}
            >
              <Input
                prefix={<UserOutlined />}
                placeholder="exemplo@email.com"
                size="large"
              />
            </Form.Item>

            <Form.Item name="senha" label="Senha" rules={[{ required: true }]}>
              <Input.Password
                prefix={<LockOutlined />}
                placeholder="******"
                size="large"
              />
            </Form.Item>

            {/* 3. Refatorado para usar Flexbox em vez de float */}
            <Form.Item>
              <div className="login-form-options">
                <Form.Item name="lembrar" valuePropName="checked" noStyle>
                  <Checkbox>Lembrar-me</Checkbox>
                </Form.Item>
                <a href="/recuperar-senha">Esqueceu sua senha?</a>
              </div>
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={isLoading}
                block
                size="large"
              >
                Entrar
              </Button>
            </Form.Item>
          </Form>
        </div>
      </Col>
    </Row>
  );
};

export default Login;
