import React from "react";
import { useAuth } from "../../../services/AuthContext";
import { NavLink } from "react-router-dom";
import { Menu, Typography } from "antd";

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

  const menuItems = [
    {
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
    {
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
    {
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
    {
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
  ];

  return (
    <div className="sidebar-container">
      {/* 2. Aplica as classes CSS para o header e o estado 'collapsed' */}
      <div className={`sidebar-header ${collapsed ? "collapsed" : ""}`}>
        {!collapsed && (
          <div className="flex flex-col items-center">
            <Text className="clinic-name">{nomeClinica || "Clínica"}</Text>
            <Text className="user-name">{userName || "Usuário"}</Text>
          </div>
        )}
      </div>

      <Menu
        theme="dark"
        mode="inline"
        defaultSelectedKeys={[window.location.pathname]}
        selectedKeys={[window.location.pathname]}
        className="sidebar-menu"
        items={menuItems} // 🔑 agora usa items em vez de children
      />
    </div>
  );
};

export default Sidebar;
