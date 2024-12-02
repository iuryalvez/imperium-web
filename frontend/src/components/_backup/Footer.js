import React from 'react';

import logoUfg from "../assets/UFG_logo.png";
import logoEvz from "../assets/EVZ_UFG.svg";
import logoInf from "../assets/INF_UFG.svg";

import '../App.css';
import './Footer.css'; // Importa o arquivo de estilos para o footer

function Footer() {
  return (
    <footer className='footer'>
      <div className='logos-container'>
        <img className='logos' src={logoUfg} alt='Logo UFG'/>
        <img className='logos' src={logoEvz} alt='Logo EVZ'/>
        <img className='logos' src={logoInf} alt='Logo INF'/>
      </div>
    </footer>
  );
}

export default Footer;
