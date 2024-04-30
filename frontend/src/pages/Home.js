import React from 'react';
import { Link } from 'react-router-dom';
import '../components/Navbar.css'
import '../components/Button.css'
import logo from '../assets/logo-unificada.png';
import ufg from '../assets/evz-ufg.png';

function Navbar({ user, handleLogout }) {
  const date = new Date().toLocaleDateString();

  return (
    <nav className="navbar">
      <div className="container">
        <div className="navbar-brand">
            <img src={logo} alt="Logo" style={{ width: '125px', height: 'auto'}} />
        </div>
        <div className="navbar-user">
            <span>Olá, nome_de_usuario!</span>
        </div>
        <div className="navbar-menu">
          <Link to="/sobre">Sobre</Link>
          <button onClick={handleLogout}>Sair </button>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
