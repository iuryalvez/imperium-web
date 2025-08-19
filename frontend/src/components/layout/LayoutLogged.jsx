// src/components/layout/LayoutLogged.jsx (Refatorado)
import React from 'react';
import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import AppHeader from './Header/AppHeader.jsx';

const { Content } = Layout;

const LayoutLogged = () => {
  return (
    <Layout>
      <AppHeader />
      {/* O Content já vem com um espaçamento padrão e pode ser customizado */}
      <Content style={{ paddingRight: '4rem', paddingLeft: '4rem', paddingTop: '2rem', marginTop: 64, minHeight: 'calc(100vh - 64px)' }}>
        {/* marginTop: 64px é a altura padrão do Header do Ant Design */}
        <Outlet />
      </Content>
    </Layout>
  );
};

export default LayoutLogged;