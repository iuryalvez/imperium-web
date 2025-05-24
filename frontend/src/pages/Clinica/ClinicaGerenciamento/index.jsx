import React from 'react';
import { useNavigate } from 'react-router-dom';

import '../../../components/styles/Gerenciamento.css';

function ClinicaGerenciamento() {
  const navigate = useNavigate();
  return (
    <div>
        <main className='main-content'>
          <h2>ClinicaGerenciamento</h2>
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

export default ClinicaGerenciamento;
