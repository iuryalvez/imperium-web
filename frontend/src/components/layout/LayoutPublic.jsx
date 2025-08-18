// src/components/layout/LayoutPublic.jsx (Refatorado)
import React from 'react';
import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import AppHeader from './Header/AppHeader.jsx';
import { colors } from '../../theme/colors.js';

const { Content } = Layout;

const LayoutPublic = () => {
  return (
    <Layout>
      <AppHeader />
      {/* Para uma página de login, talvez você não queira padding */}
      <Content style={{ marginTop: 64, backgroundColor: colors.verde.fraco, minHeight: 'calc(100vh - 64px)' }}>
        <Outlet />
      </Content>
    </Layout>
  );
};

export default LayoutPublic;