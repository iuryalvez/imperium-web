import React from "react";
import { useAuth } from "../../../services/AuthContext";
import { NavLink } from "react-router-dom";
import { Menu, Tooltip, Typography } from "antd";
import { HomeOutlined } from "@ant-design/icons";
import { usePermissao } from "../../../utils/usePermissao";

import ClinicaIcon from "../../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import AnimaisIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import TutoresIcon from "../../../assets/client/client-profile-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";
import RacoesIcon from "../../../assets/animal/dog-food-pet-svgrepo-com.png";

import "./Sidebar.css";

const { Text } = Typography;

const Sidebar = ({ collapsed, toggleCollapse }) => {
  const { userName, nomeClinica } = useAuth();

  // Cada item aparece só para os perfis com a permissão (utils/permissoes.js, espelho das policies do backend).
  // O admin simulando acesso vê o que os perfis escolhidos na simulação permitem.
  const pode = usePermissao();

  const menuItems = [
    {
      key: "/clinica/inicio",
      icon: <HomeOutlined className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`} />,
      label: <NavLink to="/clinica/inicio">Meu Painel</NavLink>,
    },
    pode("gerenciaClinica") && {
      key: "/clinica/editar",
      icon: (
        <img
          src={ClinicaIcon}
          alt="Clínica"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/editar">Clínica</NavLink>,
    },
    pode("visualizaAtendimento") && {
      key: "/clinica/tutores",
      icon: (
        <img
          src={TutoresIcon}
          alt="Tutores"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/tutores">Tutores</NavLink>,
    },
    {
      key: "/clinica/animais",
      icon: (
        <img
          src={AnimaisIcon}
          alt="Animais"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/animais">Animais</NavLink>,
    },
    {
      key: "/clinica/racoes",
      icon: (
        <img
          src={RacoesIcon}
          alt="Rações"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/racoes">Rações</NavLink>,
    },
    pode("visualizaAtendimento") && {
      key: "/clinica/consultas",
      icon: (
        <img
          src={ConsultasIcon}
          alt="Consultas"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/consultas">Consultas</NavLink>,
    },
    pode("gerenciaClinica") && {
      key: "/clinica/usuarios",
      icon: (
        <img
          src={UsuariosIcon}
          alt="Usuários"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
      label: <NavLink to="/clinica/usuarios">Usuários</NavLink>,
    },
  ].filter(Boolean);

  return (
    <div className="sidebar-container">
      <div className={`sidebar-header ${collapsed ? "collapsed" : ""}`}>
        {!collapsed && (
          // Usando Tailwind w-full e overflow-hidden para garantir que o contêiner não expanda
          <div className="flex flex-col items-center w-full px-2 overflow-hidden">
            {/* Tooltip mostra o nome completo caso o usuário passe o mouse sobre o texto truncado */}
            <Tooltip title={nomeClinica || "Clínica"} placement="right">
              <Text className="clinic-name text-center w-full" ellipsis={true}>
                {nomeClinica || "Clínica"}
              </Text>
            </Tooltip>

            <Tooltip title={userName || "Usuário"} placement="right">
              <Text className="user-name text-center w-full" ellipsis={true}>
                {userName || "Usuário"}
              </Text>
            </Tooltip>
          </div>
        )}
      </div>

      <Menu
        theme="dark"
        mode="inline"
        defaultSelectedKeys={[window.location.pathname]}
        selectedKeys={[window.location.pathname]}
        className="sidebar-menu"
        items={menuItems}
      />
    </div>
  );
};

export default Sidebar;
