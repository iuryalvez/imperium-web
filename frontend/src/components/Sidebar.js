import React from 'react';
import { NavLink } from 'react-router-dom';
import { FaTachometerAlt, FaPaw, FaCalendarAlt, FaUserFriends, FaCog, FaHome } from 'react-icons/fa';
import './Sidebar.css';

function Sidebar() {
  return (
    <aside className='sidebar'>
      <ul className='sidebar-menu'>
        <li className='menu-item'>
          <NavLink to="/home" activeClassName="active">
            <FaHome className="menu-icon" />
            Home
          </NavLink>
        </li>
        <li className='menu-item'>
          <NavLink to="/dashboard" activeClassName="active">
            <FaTachometerAlt className="menu-icon" />
            Dashboard
          </NavLink>
        </li>
        <li className='menu-item'>
          <NavLink to="/tutores" activeClassName="active">
            <FaUserFriends className="menu-icon" />
            Tutores
          </NavLink>
        </li>
        <li className='menu-item'>
          <NavLink to="/animais" activeClassName="active">
            <FaPaw className="menu-icon" />
            Animais
          </NavLink>
        </li>
        <li className='menu-item'>
          <NavLink to="/consultas" activeClassName="active">
            <FaCalendarAlt className="menu-icon" />
            Consultas
          </NavLink>
        </li>
        <li className='menu-item'>
          <NavLink to="/gerenciamento" activeClassName="active">
            <FaCog className="menu-icon" />
            Gerenciamento
          </NavLink>
        </li>
      </ul>
    </aside>
  );
}

export default Sidebar;
