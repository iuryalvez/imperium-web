import React from 'react';
import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import AppHeader from './Header/AppHeader.jsx';
import AppFooter from './Footer/AppFooter.jsx';
import './AppShell.css';

const { Content } = Layout;

// Telas públicas (Login, Sobre, Recuperar senha): o mesmo shell das telas logadas, com o
// fundo verde-fraco. Cada tela cuida do próprio espaçamento (o conteúdo não tem padding).
const LayoutPublic = () => {
  return (
    <Layout className="app-shell">
      <AppHeader />
      <Layout className="app-shell-principal">
        <Content className="app-shell-conteudo app-shell-conteudo-publico">
          <Outlet />
        </Content>
        <AppFooter />
      </Layout>
    </Layout>
  );
};

export default LayoutPublic;
