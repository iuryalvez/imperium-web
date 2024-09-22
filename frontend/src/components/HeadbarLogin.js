import React from 'react';
import { Link } from 'react-router-dom';
import './HeadbarLogin.css';
import logo from '../assets/logo-unificada.png';

function Navbar() {
    return (
        <nav className='navbar'>
            <div className='container'>
            <div className='navbar-item'>
                <img className='logo' src={logo} alt="Logo"/>
            </div>
            <div className='navbar-menu'>
                <Link to="/sobre">Sobre</Link>
                <p className='divisoria'>|</p>
                <Link to="/cadastro">Cadastro</Link>
                <p className='divisoria'>|</p>
                <Link to="/login">Login</Link>
            </div>
            </div>
        </nav>
    );
}

export default Navbar;
