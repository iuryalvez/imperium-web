import React from "react";
import { useAuth } from "../../../services/AuthContext";
import { NavLink } from "react-router-dom";
import { Menu, Typography, Button } from "antd";
import { MenuFoldOutlined, MenuUnfoldOutlined } from "@ant-design/icons";

import ClinicaIcon from "../../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import AnimaisIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import TutoresIcon from "../../../assets/client/client-profile-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";
import RacoesIcon from "../../../assets/animal/dog-food-pet-svgrepo-com.png";

const { Text } = Typography;

const SidebarDrawer = ({ collapsed, toggleCollapse }) => {
  const { userName, nomeClinica } = useAuth();

  const menuItems = [
    {
      key: "/clinica/editar",
      icon: ClinicaIcon,
      label: "Clínica",
      path: "/clinica/editar"
    },
    {
      key: "/clinica/tutores",
      icon: TutoresIcon,
      label: "Tutores",
      path: "/clinica/tutores"
    },
    {
      key: "/clinica/animais",
      icon: AnimaisIcon,
      label: "Animais",
      path: "/clinica/animais"
    },
    {
      key: "/clinica/racoes",
      icon: RacoesIcon,
      label: "Rações",
      path: "/clinica/racoes"
    },
    {
      key: "/clinica/consultas",
      icon: ConsultasIcon,
      label: "Consultas",
      path: "/clinica/consultas"
    },
    {
      key: "/clinica/usuarios",
      icon: UsuariosIcon,
      label: "Usuários",
      path: "/clinica/usuarios"
    },
  ];
  
  return (
    <div className={`h-screen fixed h-full shadow-sm transition-all duration-300 ease-in-out ${collapsed ? 'w-20' : 'w-64'}`}>
      {/* Botão de toggle */}
      <Button
        type="text"
        onClick={toggleCollapse}
        className={`absolute -right-10 w-10 h-10 flex items-center justify-center bg-[#006634] text-white rounded-r-lg hover:bg-[#00552b]`}
      >
        {collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
      </Button>

      {/* Conteúdo da Sidebar */}
      <div className={`h-full bg-[#006634] flex flex-col border-r border-[#278F54] shadow-lg`}>
        {/* Cabeçalho */}
        <div className={`p-4 flex flex-col items-center border-b border-[#278F54] transition-all duration-300 ${collapsed ? 'px-2' : 'px-4'}`}>
          <div className={`flex flex-col items-center ${collapsed ? 'w-12' : 'w-full'}`}>
            <Text className={`text-white ${collapsed ? 'text-xs truncate w-full text-center' : 'text-base'} mt-2 mb-2`}>
              {collapsed ? userName?.split(' ')[0] || "Usuário" : userName || "Usuário"}
            </Text>
            <Text className={`text-white ${collapsed ? 'text-xs truncate w-full text-center' : 'text-sm'}`}>
              {collapsed ? nomeClinica?.split(' ')[0] || "Clínica" : nomeClinica || "Clínica"}
            </Text>
          </div>
        </div>

        {/* Itens do Menu */}
        <Menu
          theme="dark"
          mode="inline"
          defaultSelectedKeys={[window.location.pathname]}
          selectedKeys={[window.location.pathname]}
          className="bg-[#006634] text-white flex-1 overflow-y-auto"
          inlineCollapsed={collapsed}
        >
          {menuItems.map((item) => (
            <Menu.Item 
              key={item.key} 
              icon={<img src={item.icon} alt={item.label} className={`${collapsed ? 'w-4 h-4' : 'w-6 h-6'}`} />}
              className="hover:bg-[#00552b]"
            >
              <NavLink
                to={item.path}
                className={({ isActive }) =>
                  isActive ? "text-[#21BA45]" : "text-white"
                }
              >
                {item.label}
              </NavLink>
            </Menu.Item>
          ))}
        </Menu>
      </div>
    </div>
  );
};

export default SidebarDrawer;