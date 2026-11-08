import React, { useMemo } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Avatar, Button, Dropdown, Layout, Menu, Spin } from "antd";
import {
  DownOutlined,
  LogoutOutlined,
  MenuOutlined,
  SwapOutlined,
  UserOutlined,
} from "@ant-design/icons";

import logo from "../../../assets/logo-unificada.png";
import { useAuth } from "../../../services/AuthContext";
import { perfisSistema } from "../../../utils/renderers";
import "./AppHeader.css";

const { Header } = Layout;

// Rótulo legível dos perfis do vínculo (ex.: "Gestor · Veterinário").
const rotuloPerfis = (perfis = []) =>
  perfis
    .map((perfil) => perfisSistema.find((p) => p.value === perfil)?.label ?? perfil)
    .map((label) => label.charAt(0) + label.slice(1).toLowerCase())
    .join(" · ");

/**
 * Header de todas as telas.
 * - Logado (com ou sem vínculo): à direita, a clínica atual e o menu do usuário
 *   (Meu perfil, Trocar vínculo, Sair).
 * - Público: links Sobre e Login.
 *
 * Props:
 * - onAbrirMenu: quando informado, mostra o botão ☰ que abre a sidebar em gaveta (celular).
 */
const AppHeader = ({ onAbrirMenu }) => {
  const {
    isLoading,
    isLogged,
    isAuthenticated,
    logout,
    trocarVinculo,
    userName,
    nomeClinica,
    perfis,
  } = useAuth();
  const navigate = useNavigate();

  const menuUsuario = useMemo(
    () => ({
      items: [
        // Identificação no topo do menu. O nome e a clínica já aparecem no header a partir de md,
        // então no menu só aparecem no celular (onde o header mostra só o avatar). Sem vínculo,
        // no desktop não sobra nada, e o bloco inteiro (com a linha abaixo) some.
        {
          key: "identificacao",
          disabled: true,
          className: `app-header-usuario-identificacao ${isAuthenticated ? "" : "app-header-so-celular"}`,
          label: (
            <div className="flex flex-col py-1">
              <span className="font-semibold text-verde-escuro md:hidden">{userName || "Usuário"}</span>
              {isAuthenticated && (
                <>
                  <span className="text-xs text-gray-500 md:hidden">{nomeClinica}</span>
                  <span className="text-xs text-gray-500">{rotuloPerfis(perfis)}</span>
                </>
              )}
            </div>
          ),
        },
        { type: "divider", className: isAuthenticated ? undefined : "app-header-so-celular" },
        {
          key: "perfil",
          icon: <UserOutlined />,
          label: "Meu perfil",
          onClick: () => navigate(isAuthenticated ? "/clinica/perfil" : "/sistema/perfil"),
        },
        isAuthenticated && {
          key: "trocar",
          icon: <SwapOutlined />,
          label: "Trocar vínculo",
          onClick: trocarVinculo,
        },
        { type: "divider" },
        {
          key: "sair",
          icon: <LogoutOutlined />,
          label: "Sair",
          danger: true,
          onClick: logout,
        },
      ].filter(Boolean),
    }),
    [userName, nomeClinica, perfis, isAuthenticated, navigate, trocarVinculo, logout]
  );

  const renderDireita = () => {
    if (isLoading) return <Spin size="small" />;

    if (isLogged) {
      return (
        <div className="app-header-direita">
          {isAuthenticated && nomeClinica && (
            <span className="app-header-clinica" title={nomeClinica}>
              {nomeClinica}
            </span>
          )}
          <Dropdown menu={menuUsuario} trigger={["click"]} placement="bottomRight">
            <button type="button" className="app-header-usuario" aria-label="Menu do usuário">
              <Avatar size={32} icon={<UserOutlined />} className="app-header-avatar" />
              <span className="app-header-usuario-nome">{userName || "Usuário"}</span>
              <DownOutlined className="text-xs text-verde-medio" />
            </button>
          </Dropdown>
        </div>
      );
    }

    return (
      <Menu
        theme="light"
        mode="horizontal"
        className="app-header-menu border-none"
        items={[
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
        ]}
      />
    );
  };

  return (
    <Header className="app-header">
      {onAbrirMenu && (
        <Button
          type="text"
          icon={<MenuOutlined />}
          onClick={onAbrirMenu}
          aria-label="Abrir menu"
          className="app-header-botao-menu"
        />
      )}
      <div className="app-header-logo cursor-pointer" onClick={() => navigate("/")}>
        <img src={logo} alt="Logo" />
      </div>

      {renderDireita()}
    </Header>
  );
};

export default AppHeader;
