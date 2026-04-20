import React, { useMemo } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Layout, Menu, Spin } from "antd";

import logo from "../../../assets/logo-unificada.png";
import "./AppHeader.css";

import { useAuth } from "../../../services/AuthContext";

const { Header } = Layout;

const AppHeader = () => {
  const { isLoading, isLogged, isAuthenticated, logout, trocarVinculo } =
    useAuth();
  const navigate = useNavigate();

  const menuItems = useMemo(() => {
    if (isLoading) {
      return [<Spin key="loading" />];
    }

    if (isAuthenticated) {
      return [
        {
          key: "trocar",
          label: <span className="app-header-menu-item">Trocar Vínculo</span>,
          onClick: trocarVinculo,
        },
        {
          key: "sair",
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
          label: (
            <Link to="/sistema/perfil" className="app-header-menu-item">
              Perfil
            </Link>
          ),
        },
        {
          key: "sair",
          label: <span className="app-header-menu-item">Sair</span>,
          onClick: logout,
          danger: true,
        },
      ];
    }

    return [
      {
        key: "sobre",
        label: (
          <Link to="/sobre" className="app-header-menu-item">
            Sobre
          </Link>
        ),
      },
      {
        key: "login",
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
      <div className="app-header-logo" onClick={() => navigate("/")}>
        <img src={logo} alt="Logo" />{" "}
      </div>

      <Menu
        theme="light"
        mode="horizontal"
        items={menuItems}
        disabled={isLoading}
        className="app-header-menu"
      />
    </Header>
  );
};

export default AppHeader;
