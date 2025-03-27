import React from "react";
import { useAuth } from "../../services/AuthContext";
import { NavLink } from "react-router-dom";
import { Layout, Menu, Typography } from "antd";

import ClinicaIcon from "../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import AnimaisIcon from "../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import TutoresIcon from "../../assets/client/client-profile-svgrepo-com.png";
import UsuariosIcon from "../../assets/user/user-group-svgrepo-com.png";
import RacoesIcon from "../../assets/animal/dog-food-pet-svgrepo-com.png";

const { Sider } = Layout;
const { Text } = Typography;

const Sidebar = () => {
  const { userName, clinicaNome } = useAuth();

  return (
    <Sider width={250} className="h-screen bg-[#006634]">
      <div className="p-4 flex flex-col items-center border-b border-[#278F54]">
        {/* <Avatar size={20} src={UsuariosIcon} /> */}
        <Text className="text-white text-base mt-2 mb-2">{userName || "Usuário"}</Text>
        <Text className="text-white text-sm">{clinicaNome || "Clínica"}</Text>
      </div>
      <Menu
        theme="white"
        mode="inline"
        defaultSelectedKeys={[window.location.pathname]}
        selectedKeys={[window.location.pathname]}
        className="bg-[#006634] text-white"
      >
        <Menu.Item key="/clinica/editar" icon={<img src={ClinicaIcon} alt="Clínica" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/editar"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Clínica
          </NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/animais" icon={<img src={AnimaisIcon} alt="Animais" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/animais"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Animais
          </NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/consultas" icon={<img src={ConsultasIcon} alt="Consultas" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/consultas"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Consultas
          </NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/tutores" icon={<img src={TutoresIcon} alt="Tutores" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/tutores"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Tutores
          </NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/usuarios" icon={<img src={UsuariosIcon} alt="Usuários" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/usuarios"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Usuários
          </NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/racoes" icon={<img src={RacoesIcon} alt="Rações" className="w-[22px] h-[22px]" />}>
          <NavLink
            to="/clinica/racoes"
            className={({ isActive }) =>
              isActive ? "text-[#21BA45]" : "text-white"
            }
          >
            Rações
          </NavLink>
        </Menu.Item>
      </Menu>
    </Sider>
  );
};

export default Sidebar;
