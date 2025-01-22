import React from 'react';

import '../App.css'
import Sidebar from '../components/molecules/Sidebar';

function Home() {
  return (
    <div>
        <Sidebar />
        <div className='main-content'>
            <h1>Bem-vindo ao sistema de gerenciamento da clínica veterinária!</h1>
            <p>Aqui você pode gerenciar todas as operações da clínica.</p>
        </div>
    </div>
  );
}

export default Home;
