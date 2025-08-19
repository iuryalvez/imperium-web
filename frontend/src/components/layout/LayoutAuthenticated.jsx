// src/components/layout/LayoutAuthenticated.jsx (Refatorado)
import React, { useState } from "react";
import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import AppHeader from "./Header/AppHeader.jsx";
import Sidebar from "./Sidebar/Sidebar.jsx";
import { colors } from "../../theme/colors.js";
import "./LayoutAuthenticated.css";

const { Content, Sider } = Layout;

const LayoutAuthenticated = () => {
  const [collapsed, setCollapsed] = useState(false);

  return (
    // O Layout principal envolve toda a página
    <Layout style={{ }}>
      <AppHeader />
      {/* Um segundo Layout para a combinação Sider + Content */}
      <Layout>
        <Sider
          collapsible
          collapsed={collapsed}
          onCollapse={(value) => setCollapsed(value)}
          width={256} // Largura padrão quando expandido (w-64 do Tailwind)
          style={{
            // Estilos para manter a sidebar fixa enquanto o conteúdo rola
            overflow: "auto",
            height: "100vh",
            position: "fixed",
            left: 0,
            top: 0,
            bottom: 0,
            marginTop: "64px",
            backgroundColor: colors.verde.medio, // Obs: Não funciona setando no ant-theme, tive que setar aqui.
          }}
        >
          {/* Nosso componente Sidebar simplificado é renderizado AQUI DENTRO */}
          <Sidebar collapsed={collapsed} />
        </Sider>

        {/* Este Layout de conteúdo se ajusta automaticamente à sidebar */}
        <Layout
          style={{
            paddingRight: '2rem',
            paddingLeft: '2rem',
            marginLeft: collapsed ? 80 : 256, // Largura padrão do antd recolhido (80) e expandido (256)
            transition: "margin-left 0.2s",
            marginTop: 64,
            backgroundColor: colors.verde.fraco,
            minHeight: 'calc(100vh - 64px)', 
          }}
        >
          <Content style={{ margin: "24px 16px", overflow: "initial" }}>
            <Outlet />
          </Content>
        </Layout>
      </Layout>
    </Layout>
  );
};

export default LayoutAuthenticated;
