import React from 'react';
import { useNavigate } from 'react-router-dom';

import '../App.css';
import './Gerenciamento.css';
import Navsidebar from '../components/Navsidebar';


function Gerenciamento() {
  const navigate = useNavigate();
  return (
    <div>
        <Navsidebar></Navsidebar>
        <main className='main-content'>
          <h2>Gerenciamento</h2>
          <div className='opcoes'>
            <div>
              <button className='button-menu' onClick={()=> navigate('/gerenciamento/usuarios')}>Usuários</button>
            </div>
            <div>
              <button className='button-menu button-desativado' onClick={()=> navigate('/gerenciamento/perfil')}>Perfil</button>
            </div>
          </div>
        </main>
    </div>
  );
}

export default Gerenciamento;
