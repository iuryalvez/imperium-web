import React from 'react';
import { Row, Col, Typography, Space } from 'antd';
import { useAuth } from '../../services/AuthContext';
import FormLogin from '../../components/organisms/FormLogin';
import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";

const { Title, Paragraph } = Typography;

const Login = () => {
  const { login } = useAuth();

  const handleLogin = async ({ email, senha }) => {
    login({ email, senha }); // chama a função login do contexto
  };

  return (
    <Row className="h-full" style={{ }}>
      {/* Lado esquerdo - informações */}
      <Col span={14} className="pt-20 pr-10 pl-20 flex flex-col justify-start text-[var(--verde-medio)]">
        <Title level={1} style={{ color: '#064e3b' }}>Bem-vindo ao Software IMPERIUM!</Title>
        <Paragraph className="text-lg" style={{ color: '#064e3b', fontSize: '16px', lineHeight: 1.75 }}>
          Este software tem como objetivo auxiliar no controle nutricional de cães e gatos, utilizando técnicas
          de <strong>inteligência artificial</strong>, em especial o <strong>aprendizado de máquina</strong>,
          para padronizar a avaliação do Escore de Condição Corporal (ECC) dos animais. Com isso, o sistema
          contribui diretamente para a promoção da saúde e do bem-estar dos pets.
        </Paragraph>

        <Space size="large" className="mt-10">
          <img src={logoUfg} alt="Logo UFG" className="h-16" />
          <img src={logoEvz} alt="Logo EVZ" className="h-16" />
          <img src={logoInf} alt="Logo INF" className="h-16" />
        </Space>
      </Col>

      {/* Lado direito - formulário */}
      <Col span={10} className="pt-20 pr-20 flex items-start justify-center">
        <FormLogin handleLogin={handleLogin} />
      </Col>
    </Row>
  );
};

export default Login;
