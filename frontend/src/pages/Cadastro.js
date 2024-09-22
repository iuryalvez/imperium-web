import React, { useState } from 'react';

import '../App.css';

import CardCadastro from '../components/CardCadastro';
import HeadbarLogin from '../components/HeadbarLogin';

const Cadastro = () => {

  return (
    <div>
      <HeadbarLogin/>
      <div className='cadastro-content'>
        <CardCadastro/>
      </div>
    </div>
  );
};

export default Cadastro;
