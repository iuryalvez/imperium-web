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
import { useAuth } from "../../../services/AuthContext";
import { Turnstile } from "@marsidev/react-turnstile";
import logoInf from "../../../assets/INF_UFG.png"
import logoEvz from "../../../assets/EVZ_UFG.png"
import logoUfg from "../../../assets/UFG_logo.png";
import logoSps from "../../../assets/sps-logo-preto.png";
import { UserOutlined, LockOutlined } from "@ant-design/icons";
import iconLogin from "../../../assets/auth/login-svgrepo-com.png";

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
      turnstileRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLocalLoading(false);
    }
  };

  return (
    <Row className="flex-1 flex items-start pt-[5vh]">
      <Col xs={24} md={14} className="p-6 sm:p-8 md:pl-16 flex flex-col">
        <Title level={1}>Bem-vindo ao Software IMPERIUM!</Title>
        <Paragraph className="text-base leading-relaxed">
          Este software tem como objetivo auxiliar no controle nutricional de
          cães e gatos, utilizando{" "}
          <strong>metodologias validadas pela literatura científica</strong>,
          como o sistema de Laflamme e as equações do NRC, para padronizar a
          avaliação do Escore de Condição Corporal (ECC) e o cálculo das
          necessidades energéticas dos animais.
        </Paragraph>

        <div className="mt-8 md:mt-20 xl:mt-auto pt-8 flex flex-col w-full border-t border-verde-escuro/10">
          <span className="text-sm font-semibold text-verde-escuro uppercase tracking-wider mb-5 opacity-70">
            Desenvolvido em parceria com:
          </span>
          <Space size={48} className="flex-wrap items-center">
            <a href="https://ufg.br/" target="_blank" rel="noopener noreferrer">
              <img src={logoUfg} alt="Logo UFG" className="h-16" />
            </a>
            <a href="https://evz.ufg.br/" target="_blank" rel="noopener noreferrer">
              <img src={logoEvz} alt="Logo EVZ" className="h-16" />
            </a>
            <a href="https://inf.ufg.br/" target="_blank" rel="noopener noreferrer">
              <img src={logoInf} alt="Logo INF" className="h-16" />
            </a>
            <a href="https://pecuariasustentavel.evz.ufg.br/" target="_blank" rel="noopener noreferrer">
              <img src={logoSps} alt="Logo SPS" className="h-16" />
            </a>
          </Space>
        </div>
      </Col>

      <Col
        xs={24}
        md={10}
        // No celular o formulário vem antes do texto de boas-vindas, para não precisar rolar até ele
        className="order-first md:order-none p-6 sm:p-8 md:pr-16 flex justify-center items-start"
      >
        <div className="flex flex-col items-center bg-white rounded-3xl shadow-lg p-6 sm:p-10 w-full max-w-[450px]">
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
                icon={
                  <Space>
                    <img
                      src={iconLogin}
                      alt="Login"
                      className="w-4 h-4 object-contain"
                    />
                  </Space>
                }
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
