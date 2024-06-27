import React from 'react';
import { Link } from 'react-router-dom';
import './Sidebar.css';

function Sidebar() {
  return (
    <aside className='sidebar'>
      <ul className='sidebar-menu'>
        <li className='menu-item'>
          <Link to="/dashboard">Dashboard</Link>
        </li>
        <li className='menu-item'>
          <Link to="/animais">Animais</Link>
        </li>
        <li className='menu-item'>
          <Link to="/consultas">Consultas</Link>
        </li>
        <li className='menu-item'>
          <Link to="/tutores">Tutores</Link>
        </li>
        <li className='menu-item'>
          <Link to="/configuracoes">Configurações</Link>
        </li>
      </ul>
    </aside>
  );
}

export default Sidebar;
