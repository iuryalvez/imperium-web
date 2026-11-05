import React from 'react';
import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import AppHeader from './Header/AppHeader.jsx';
import AppFooter from './Footer/AppFooter.jsx';
import './AppShell.css';

const { Content } = Layout;

// Layout das telas do Sistema e da escolha de vínculo: o mesmo shell da Clínica, sem sidebar
// (header e rodapé fixos; a partir de 768px só a área de conteúdo rola).
const LayoutLogged = () => {
  return (
    <Layout className="app-shell">
      <AppHeader />
      <Layout className="app-shell-principal">
        <Content className="app-shell-conteudo app-shell-conteudo-largo">
          <Outlet />
        </Content>
        <AppFooter />
      </Layout>
    </Layout>
  );
};

export default LayoutLogged;
