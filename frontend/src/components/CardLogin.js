import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { FaLock, FaUser } from 'react-icons/fa';

import Logos from './Logos';
import logo from '../assets/imperium-logo.png';
import '../App.css';
import './CardLogin.css';

const LoginForm = ({ onLogin, onShowForgotsenhaForm }) => {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  
  const handleSubmit = (e) => {
    e.preventDefault();
    onLogin(email, senha);
  };

  return (
    <form className='form-login' onSubmit={handleSubmit}>
      <img src={logo} alt="Logo da Imperium" className='logo-login' />
      <h1>LOGIN</h1>
      <div className='texto-cadastrar'>
        <p>Não tem uma conta?</p>
          <div className='lado-a-lado'>
            <p>Pode cadastrar-se</p>
            <Link className='aqui' to='/cadastro'>aqui!</Link>
          </div>
      </div>
        <label>
          E-mail
        </label>
      <div className='lado-a-lado'>
        <FaUser className='input-icon'></FaUser>
        <input className='input-form'
          type="text"
          placeholder='Seu e-mail'
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          />
      </div>
      <label>
        Senha
      </label>
      <div className='lado-a-lado'>
        <FaLock className='input-icon'></FaLock>
        <input className='input-form'
          type="password"
          placeholder='Sua senha'
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
          required
          />
      </div>
      <button className='button-login' type="submit">LOGIN</button>
      <button type="button" className="forgot-senha-button" onClick={onShowForgotsenhaForm}>Esqueceu sua senha?</button>
      <Logos />
    </form>
  );
};

export default LoginForm;
