import React from 'react';

import '../App.css'
import Navsidebar from '../components/organisms/Navsidebar';

function Home() {
  return (
    <div className='main-content'>
        <Navsidebar />
        <div className='info-content'>
            <h1>Bem-vindo ao sistema de gerenciamento da clínica veterinária!</h1>
            <p>Aqui você pode gerenciar todas as operações da clínica.</p>
        </div>
    </div>
  );
}

export default Home;