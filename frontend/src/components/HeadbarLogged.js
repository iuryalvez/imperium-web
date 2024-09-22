import React from 'react';
import { Link } from 'react-router-dom';

import '../App.css';
import './HeadbarLogged.css';

import logo from '../assets/logo-unificada.png';

function Navbar() {
    return (
        <nav className='navbar'>
            <div className='container'>
            <div className='navbar-item'>
                <img className='logo' src={logo} alt="Logo"/>
            </div>
            <div className='navbar-item'>
                <span>Olá!</span>
            </div>
            <div className='navbar-menu'>
                <Link to="/perfil">Perfil</Link>
                <p>|</p>
                <Link to="/login">Sair</Link>
            </div>
            </div>
        </nav>
    );
}

export default Navbar;
