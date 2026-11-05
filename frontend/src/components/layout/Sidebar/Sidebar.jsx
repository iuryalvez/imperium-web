import React from "react";
import { NavLink, useLocation } from "react-router-dom";
import { Menu } from "antd";
import { usePermissao } from "../../../utils/usePermissao";

import MeuPainelIcon from "../../../assets/animal/pet-hotel-sign-of-a-bone-in-a-dog-house-svgrepo-com.png";
import ClinicaIcon from "../../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import AnimaisIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import TutoresIcon from "../../../assets/client/client-profile-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";
import RacoesIcon from "../../../assets/animal/dog-food-pet-svgrepo-com.png";

import "./Sidebar.css";

const Sidebar = ({ collapsed }) => {
  // useLocation já vem sem o basename (/dev em homologação), ao contrário de window.location.
  const { pathname } = useLocation();

  // Cada item aparece só para os perfis com a permissão (utils/permissoes.js, espelho das policies do backend).
  // O admin simulando acesso vê o que os perfis escolhidos na simulação permitem.
  const pode = usePermissao();

  const menuItems = [
    {
      key: "/clinica/inicio",
      icon: (
        <img
          src={MeuPainelIcon}
          alt="Meu Painel"
          className={`sidebar-menu-icon ${collapsed ? "collapsed" : ""}`}
        />
      ),
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
      <Menu
        theme="dark"
        mode="inline"
        selectedKeys={[pathname]}
        className="sidebar-menu"
        items={menuItems}
      />
    </div>
  );
};

export default Sidebar;
