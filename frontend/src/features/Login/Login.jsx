import React, { useState, useRef } from "react";
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
import { Turnstile } from "@marsidev/react-turnstile";
import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";
import { UserOutlined, LockOutlined } from "@ant-design/icons";

import "./Login.css";

const { Title, Paragraph } = Typography;

const Login = () => {
  const { login } = useAuth();
  const [localLoading, setLocalLoading] = useState(false);
  const [captchaToken, setCaptchaToken] = useState(null);
  const turnstileRef = useRef(null);

  const handleLogin = async (values) => {
    if (!captchaToken) return;

    setLocalLoading(true); // Local state for button loading UI

    try {
      await login({
        email: values.email,
        senha: values.senha,
        captchaToken,
        rememberMe: values.lembrar,
      });
    } catch (error) {
      // Executes only because AuthContext re-threw the error
      turnstileRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLocalLoading(false);
    }
  };

  return (
    <Row className="login-page-row">
      {/* xs=24 (100% em mobile), md=14 (telas médias+) */}
      <Col xs={24} md={14} className="login-info-col">
        <Title level={1}>Bem-vindo ao Software IMPERIUM!</Title>
        <Paragraph>
          Este software tem como objetivo auxiliar no controle nutricional de
          cães e gatos, utilizando técnicas de{" "}
          <strong>inteligência artificial</strong>, em especial o{" "}
          <strong>aprendizado de máquina</strong>, para padronizar a avaliação
          do Escore de Condição Corporal (ECC) dos animais.
        </Paragraph>

        <Space size="large" className="login-logos-container">
          <img src={logoUfg} alt="Logo UFG" />
          <img src={logoEvz} alt="Logo EVZ" />
          <img src={logoInf} alt="Logo INF" />
        </Space>
      </Col>

      <Col xs={24} md={10} className="login-form-col">
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

            <Form.Item>
              <div className="login-form-options">
                <Form.Item name="lembrar" valuePropName="checked" noStyle>
                  <Checkbox>Lembrar-me</Checkbox>
                </Form.Item>
                <a href="/recuperar-senha">Esqueceu sua senha?</a>
              </div>
            </Form.Item>

            <Form.Item style={{ display: "flex", justifyContent: "center" }}>
              <Turnstile
                ref={turnstileRef}
                siteKey={process.env.REACT_APP_TURNSTILE_SITE_KEY}
                onSuccess={setCaptchaToken}
                onError={() => setCaptchaToken(null)}
                onExpire={() => setCaptchaToken(null)}
                options={{ theme: "light" }}
              />
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={localLoading}
                block
                size="large"
                disabled={!captchaToken}
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
