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
      icon: ClinicaIcon,
      label: "Clínica",
      path: "/clinica/editar",
    },
    {
      key: "/clinica/tutores",
      icon: TutoresIcon,
      label: "Tutores",
      path: "/clinica/tutores",
    },
    {
      key: "/clinica/animais",
      icon: AnimaisIcon,
      label: "Animais",
      path: "/clinica/animais",
    },
    {
      key: "/clinica/racoes",
      icon: RacoesIcon,
      label: "Rações",
      path: "/clinica/racoes",
    },
    {
      key: "/clinica/consultas",
      icon: ConsultasIcon,
      label: "Consultas",
      path: "/clinica/consultas",
    },
    {
      key: "/clinica/usuarios",
      icon: UsuariosIcon,
      label: "Usuários",
      path: "/clinica/usuarios",
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
      >
        {menuItems.map((item) => (
          <Menu.Item
            key={item.key}
            icon={
              <img
                src={item.icon}
                alt={item.label}
                className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
              />
            }
          >
            {/* O NavLink não precisa mais de classe de estilo para o estado ativo */}
            <NavLink to={item.key}>{item.label}</NavLink>
          </Menu.Item>
        ))}
      </Menu>
    </div>
  );
};

export default Sidebar;
