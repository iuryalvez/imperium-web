import React from "react";
import { useAuth } from "../../services/AuthContext";
import { NavLink } from "react-router-dom";
import { Layout, Menu, Avatar, Typography } from "antd";

import ClinicaIcon from "../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import AnimaisIcon from "../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import TutoresIcon from "../../assets/client/client-profile-svgrepo-com.png";
import UsuariosIcon from "../../assets/user/user-group-svgrepo-com.png";
import RacoesIcon from "../../assets/animal/dog-food-pet-svgrepo-com.png";

const { Sider } = Layout;
const { Title, Text } = Typography;

const Sidebar = () => {
  const { userName, clinicaNome } = useAuth();

  return (
    <Sider width={250} theme="dark" className="h-screen bg-[#006634]">
      <div className="p-4 flex flex-col items-center border-b border-[#064420]">
        <Avatar size={64} src={UsuariosIcon} />
        <Title level={4} className="text-white mt-2">{userName || "Usuário"}</Title>
        <Text className="text-[#D9FFEC] text-sm">{clinicaNome || "Clínica"}</Text>
      </div>
      <Menu
        theme="white"
        mode="inline"
        defaultSelectedKeys={[window.location.pathname]}
        selectedKeys={[window.location.pathname]}
        className="bg-[#006634] text-white"
      >
        <Menu.Item key="/clinica/editar" icon={<img src={ClinicaIcon} alt="Clínica" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/editar">Clínica</NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/animais" icon={<img src={AnimaisIcon} alt="Animais" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/animais">Animais</NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/consultas" icon={<img src={ConsultasIcon} alt="Consultas" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/consultas">Consultas</NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/tutores" icon={<img src={TutoresIcon} alt="Tutores" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/tutores">Tutores</NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/usuarios" icon={<img src={UsuariosIcon} alt="Usuários" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/usuarios">Usuários</NavLink>
        </Menu.Item>
        <Menu.Item key="/clinica/racoes" icon={<img src={RacoesIcon} alt="Rações" className="w-[22px] h-[22px]" />}>
          <NavLink to="/clinica/racoes">Rações</NavLink>
        </Menu.Item>
      </Menu>
    </Sider>
  );
};

export default Sidebar;
