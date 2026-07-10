import React, { useState, useRef } from "react";
import { Row, Col, Typography, Space, Form, Button, Input, Checkbox } from "antd";
import { useAuth } from "../../services/AuthContext";
import { Turnstile } from "@marsidev/react-turnstile";
import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";
import { UserOutlined, LockOutlined } from "@ant-design/icons";

const { Title, Paragraph } = Typography;

const Login = () => {
  const { login } = useAuth();
  const [localLoading, setLocalLoading] = useState(false);
  const [captchaToken, setCaptchaToken] = useState(null);
  const turnstileRef = useRef(null);

  const handleLogin = async (values) => {
    if (!captchaToken) return;

    setLocalLoading(true);

    try {
      await login({
        email: values.email,
        senha: values.senha,
        captchaToken,
        rememberMe: values.lembrar,
      });
    } catch (error) {
      // Reset Turnstile token on authentication failure to prevent reuse
      turnstileRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLocalLoading(false);
    }
  };

  return (
    <Row className="min-h-[calc(100vh-64px)] flex items-start pt-[5vh] bg-verde-fraco">
      <Col xs={24} md={14} className="p-8 md:pl-16 flex flex-col">
        <Title level={1}>Bem-vindo ao Software IMPERIUM!</Title>
        <Paragraph className="text-base leading-relaxed">
          Este software tem como objetivo auxiliar no controle nutricional de
          cães e gatos, utilizando técnicas de <strong>inteligência artificial</strong>, 
          em especial o <strong>aprendizado de máquina</strong>, para padronizar a avaliação
          do Escore de Condição Corporal (ECC) dos animais.
        </Paragraph>

        <Space size="large" className="mt-10 flex-wrap justify-center">
          <img src={logoUfg} alt="Logo UFG" className="h-16" />
          <img src={logoEvz} alt="Logo EVZ" className="h-16" />
          <img src={logoInf} alt="Logo INF" className="h-16" />
        </Space>
      </Col>

      <Col xs={24} md={10} className="p-8 md:pr-16 flex justify-center items-start">
        <div className="flex flex-col items-center bg-white rounded-3xl shadow-lg p-10 w-full max-w-[450px]">
          <Title level={2} className="text-verde-medio mb-6">
            LOGIN
          </Title>

          <Form
            name="login"
            onFinish={handleLogin}
            layout="vertical"
            className="w-full"
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
              <div className="flex justify-between items-center w-full flex-wrap">
                <Form.Item name="lembrar" valuePropName="checked" noStyle>
                  <Checkbox>Lembrar-me</Checkbox>
                </Form.Item>
                <a href="/recuperar-senha">Esqueceu sua senha?</a>
              </div>
            </Form.Item>

            <Form.Item className="flex justify-center">
              <Turnstile
                ref={turnstileRef}
                siteKey={process.env.REACT_APP_TURNSTILE_SITE_KEY}
                onSuccess={setCaptchaToken}
                onError={() => setCaptchaToken(null)}
                onExpire={() => setCaptchaToken(null)}
                options={{ theme: "light" }}
              />
            </Form.Item>

            <Form.Item className="mb-0">
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