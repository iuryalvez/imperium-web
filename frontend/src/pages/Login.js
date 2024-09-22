import React from 'react'; // Importa o React e a função useState para gerenciar estados.
import axios from 'axios';

import '../App.css'; // Importa o arquivo de estilos globais.
import './Login.css'; // Importa o arquivo de estilos globais.

import CardLogin from '../components/CardLogin'; // Importa o componente de formulário de login.
import HeadbarLogin from '../components/HeadbarLogin';
import { Navigate } from 'react-router-dom';

function Login() {
  
  // Função para lidar com o login.
  const handleLogin = async (email, senha) => {
    try {
        const response = await axios.post('http://localhost:5050/api/Usuarios/login', {
            email,
            senha
        });
        const { token } = response.data;
        localStorage.setItem('jwtToken', token);
        Navigate('/');
    } catch (error) {
        console.error('Falha no login', error);
    }
  };

  return (
    <div>  
      <HeadbarLogin/>
      <div className='login-content'>  
        <CardLogin onLogin={handleLogin} />
      </div>
    </div>
  );
}

export default Login; // Exporta o componente Login para uso em outras partes do aplicativo.
