import React from 'react';

import '../App.css'
import Sidebar from '../components/molecules/Sidebar';

function Clinica() {
  return (
    <div>
        <Sidebar />
        <h1>Bem-vindo ao sistema de gerenciamento da clínica veterinária!</h1>
        <p>Aqui você pode gerenciar todas as operações da clínica.</p>
    </div>
  );
}

export default Clinica;
