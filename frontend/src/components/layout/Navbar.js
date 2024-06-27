import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';
import logo from '../../assets/logo-unificada.png';

function Navbar() {
    return (
        <nav className='navbar'>
            <div className='container'>
            <div className='navbar-item'>
                <img src={logo} alt="Logo" style={{ width: '125px', height: 'auto' }} />
            </div>
            <div className='navbar-item'>
                <span>Olá!</span>
            </div>
            <div className='navbar-menu'>
                <Link to="/sobre">Sobre</Link>
                <Link to="/login">Sair</Link>
            </div>
            </div>
        </nav>
    );
}

export default Navbar;
