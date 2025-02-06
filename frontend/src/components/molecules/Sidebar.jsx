import React from 'react';
import { useAuth } from '../../services/AuthContext'; 
import { NavLink } from 'react-router-dom';

import ClinicaIcon from '../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png';
import AnimaisIcon from '../../assets/animal/pet-14-svgrepo-com.png';
import ConsultasIcon from '../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png';
import TutoresIcon from '../../assets/client/client-profile-svgrepo-com.png';
import UsuariosIcon from '../../assets/user/user-group-svgrepo-com.png';
import RacoesIcon from '../../assets/animal/dog-food-pet-svgrepo-com.png';

function Sidebar() {
  const { userName, clinicaNome } = useAuth(); 
  return (
    <aside className="bg-green-600 h-screen w-64 text-white flex flex-col">
      {/* Cabeçalho */}
      <div className="w-full p-4 border-b border-green-700">
        <h2 className="text-xl font-bold">{userName || 'Usuário'}</h2>
        <p className="text-sm text-green-200">{clinicaNome || 'Clínica'}</p>
      </div>

      {/* Menu */}
      <ul className="flex flex-col mt-4 space-y-2">
        <li>
          <NavLink
            to="/clinica/editar"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={ClinicaIcon} alt="Clinica" className="mr-2 w-[22px] h-[22px]" />
            Clínica
          </NavLink>
        </li>
        <li>
          <NavLink
            to="/clinica/animais"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={AnimaisIcon} alt="Animais" className="mr-2 w-[22px] h-[22px]" />
            Animais
          </NavLink>
        </li>
        <li>
          <NavLink
            to="/clinica/consultas"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={ConsultasIcon} alt="Consultas" className="mr-2 w-[22px] h-[22px]" />
            Consultas
          </NavLink>
        </li>
        <li>
          <NavLink
            to="/clinica/tutores"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={TutoresIcon} alt="Tutores" className="mr-2 w-[22px] h-[22px]" />
            Tutores
          </NavLink>
        </li>
        <li>
          <NavLink
            to="/clinica/usuarios"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={UsuariosIcon} alt="Usuários" className="mr-2 w-[22px] h-[22px]" />
            Usuários
          </NavLink>
        </li>
        <li>
          <NavLink
            to="/clinica/racoes"
            className={({ isActive }) =>
              `flex items-center pl-10 pr-4 py-2 text-white hover:bg-green-700 transition ${
                isActive ? 'bg-green-700' : ''
              }`
            }
          >
            <img src={RacoesIcon} alt="Rações" className="mr-2 w-[22px] h-[22px]" />
            Rações
          </NavLink>
        </li>
      </ul>
    </aside>
  );
}

export default Sidebar;
