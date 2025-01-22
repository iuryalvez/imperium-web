import React from 'react';
import { useAuth } from '../services/AuthContext';

import TemplateLogin from '../components/template/TemplateLogin';

const Login = () => {
  const { login } = useAuth();

  const handleLogin = async ({ email, senha }) => {
    login({ email, senha });  // Chama a função login do contexto
  };

  return (
    <div className='login-content'> 
      <TemplateLogin handleLogin={handleLogin} />
    </div>
  );
};

export default Login;
