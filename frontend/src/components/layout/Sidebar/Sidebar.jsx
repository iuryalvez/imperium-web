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
    // Removemos o div com 'position: fixed' e o botão de toggle
    <div className="h-full flex flex-col">
      {/* Cabeçalho do Sidebar */}
      <div
        className={`p-4 flex flex-col items-center border-b border-[#278F54] transition-opacity duration-300 ${
          collapsed ? "opacity-0 h-0 p-0" : "opacity-100"
        }`}
      >
        {/* Adicionado um container para esconder o texto quando recolhido */}
        {!collapsed && (
          <div className="flex flex-col items-center">
            <Text className="text-white text-base mt-2 mb-2">
              {userName || "Usuário"}
            </Text>
            <Text className="text-white text-sm">
              {nomeClinica || "Clínica"}
            </Text>
          </div>
        )}
      </div>

      {/* Itens do Menu */}
      <Menu
        theme="dark"
        mode="inline"
        defaultSelectedKeys={[window.location.pathname]}
        selectedKeys={[window.location.pathname]}
        className="bg-transparent border-r-0" // Fundo transparente para herdar do Sider
      >
        {menuItems.map((item) => (
          <Menu.Item
            key={item.key}
            icon={<img src={item.icon} alt={item.label} className="w-4 h-4" />} // Tamanho de ícone consistente
          >
            {/* O NavLink é o conteúdo do label para navegação de SPA */}
            <NavLink to={item.key}>{item.label}</NavLink>
          </Menu.Item>
        ))}
      </Menu>
    </div>
  );
};

export default Sidebar;
