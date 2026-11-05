import React, { useEffect, useState } from "react";
import { Drawer, Grid, Layout } from "antd";
import { Outlet, useLocation } from "react-router-dom";
import AppHeader from "./Header/AppHeader.jsx";
import AppFooter from "./Footer/AppFooter.jsx";
import Sidebar from "./Sidebar/Sidebar.jsx";
import { colors } from "../../theme/colors.js";
import "./AppShell.css";
import "./LayoutAuthenticated.css";

const { Content, Sider } = Layout;

/**
 * Shell das telas da Clínica.
 * - >= lg (992px): sidebar expandida (pode recolher para ícones).
 * - md a lg (768-991px): sidebar começa recolhida, só com ícones.
 * - >= md: a página nunca rola; header, sidebar e rodapé ficam fixos e quem rola é a área de
 *   conteúdo (ou a tabela, dentro da PaginaPadrao).
 * - < md (celular): a sidebar vira gaveta (botão ☰ no header) e a página rola com o header sticky.
 */
const LayoutAuthenticated = () => {
  const telas = Grid.useBreakpoint();
  // No primeiro render o useBreakpoint ainda não mediu a tela; assume desktop para não piscar.
  const telaGrande = telas.lg ?? true;
  const telaCelular = telas.md === false;

  const [collapsed, setCollapsed] = useState(!telaGrande);
  const [menuAberto, setMenuAberto] = useState(false);
  const { pathname } = useLocation();

  // Ao cruzar 992px, a sidebar expande (desktop) ou recolhe para ícones (tablet).
  useEffect(() => setCollapsed(!telaGrande), [telaGrande]);

  // Fecha a gaveta ao navegar e ao sair do tamanho de celular.
  useEffect(() => setMenuAberto(false), [pathname, telaCelular]);

  return (
    <Layout className="app-shell">
      <AppHeader onAbrirMenu={telaCelular ? () => setMenuAberto(true) : undefined} />

      <Layout className="app-shell-corpo">
        {telaCelular ? (
          <Drawer
            placement="left"
            open={menuAberto}
            onClose={() => setMenuAberto(false)}
            width={256}
            closable={false}
            styles={{ body: { padding: 0, backgroundColor: colors.verde.medio } }}
          >
            <Sidebar collapsed={false} />
          </Drawer>
        ) : (
          <Sider
            collapsible
            collapsed={collapsed}
            onCollapse={(value) => setCollapsed(value)}
            width={256}
            className="app-shell-sider"
            style={{ backgroundColor: colors.verde.medio }} // Obs: não funciona setando no ant-theme.
          >
            <Sidebar collapsed={collapsed} />
          </Sider>
        )}

        <Layout className="app-shell-principal">
          <Content className="app-shell-conteudo">
            <Outlet />
          </Content>
          <AppFooter />
        </Layout>
      </Layout>
    </Layout>
  );
};

export default LayoutAuthenticated;
