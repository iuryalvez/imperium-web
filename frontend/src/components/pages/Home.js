import React from 'react';
import Navsidebar from '../layout/Navsidebar'
import '../../App.css'

function Home() {
  return (
    <div>
      <Navsidebar></Navsidebar>
      <main className='main-content'>
        <h1 className='h1'>Bem-vindo ao sistema de gerenciamento da clínica veterinária!</h1>
        <p className='content'>Aqui você pode gerenciar todas as operações da clínica.</p>
      </main>
    </div>
  );
}

export default Home;
