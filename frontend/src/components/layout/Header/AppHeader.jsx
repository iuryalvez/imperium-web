import React, { useMemo } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Layout, Menu, Spin } from "antd";
// import { InfoCircleOutlined, LoginOutlined } from "@ant-design/icons";

import logo from "../../../assets/logo-unificada.png";
// import iconeSair from "../../../assets/auth/logout-svgrepo-com.png";
// import iconePerfil from "../../../assets/client/client-profile-svgrepo-com.png";
// import iconeTrocar from "../../../assets/client/globe-user-svgrepo-com.png";

import "./AppHeader.css";
import { useAuth } from "../../../services/AuthContext";

const { Header } = Layout;

const AppHeader = () => {
  const { isLoading, isLogged, isAuthenticated, logout, trocarVinculo } =
    useAuth();
  const navigate = useNavigate();

  const menuItems = useMemo(() => {
    if (isLoading) {
      return [{ key: "loading", label: <Spin /> }];
    }

    if (isAuthenticated) {
      return [
        {
          key: "trocar",
          // icon: (
          //   <img
          //     src={iconeTrocar}
          //     alt="Trocar"
          //     className="w-4 h-4 object-contain"
          //   />
          // ),
          label: <span className="app-header-menu-item">Trocar Vínculo</span>,
          onClick: trocarVinculo,
        },
        {
          key: "sair",
          // icon: (
          //   <img
          //     src={iconeSair}
          //     alt="Sair"
          //     className="w-4 h-4 object-contain"
          //   />
          // ),
          label: <span className="app-header-menu-item">Sair</span>,
          onClick: logout,
          danger: true,
        },
      ];
    }

    if (isLogged) {
      return [
        {
          key: "perfil",
          // icon: (
          //   <img
          //     src={iconePerfil}
          //     alt="Perfil"
          //     className="w-4 h-4 object-contain"
          //   />
          // ),
          label: (
            <Link to="/sistema/perfil" className="app-header-menu-item">
              Perfil
            </Link>
          ),
        },
        {
          key: "sair",
          // icon: (
          //   <div
          //     className="w-4 h-4 bg-[var(--vermelho-delete)]"
          //     style={{
          //       WebkitMask: `url(${iconeSair}) center/contain no-repeat`,
          //       mask: `url(${iconeSair}) center/contain no-repeat`,
          //     }}
          //     aria-label="Sair"
          //   />
          // ),
          label: <span className="app-header-menu-item">Sair</span>,
          onClick: logout,
          danger: true,
        },
      ];
    }

    return [
      {
        key: "sobre",
        // icon: <InfoCircleOutlined />,
        label: (
          <Link to="/sobre" className="app-header-menu-item">
            Sobre
          </Link>
        ),
      },
      {
        key: "login",
        // icon: <LoginOutlined />,
        label: (
          <Link to="/login" className="app-header-menu-item">
            Login
          </Link>
        ),
      },
    ];
  }, [isLoading, isAuthenticated, isLogged, logout, trocarVinculo]);

  return (
    <Header className="app-header">
      <div
        className="app-header-logo cursor-pointer"
        onClick={() => navigate("/")}
      >
        <img src={logo} alt="Logo" />
      </div>

      <Menu
        theme="light"
        mode="horizontal"
        items={menuItems}
        disabled={isLoading}
        className="app-header-menu border-none"
      />
    </Header>
  );
};

export default AppHeader;
