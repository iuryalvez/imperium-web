import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';  // Importa o axios para fazer requisições HTTP

import { FaEnvelope, FaLock, FaUser } from 'react-icons/fa';

import '../App.css';
import './CardCadastro.css';
import logo from '../assets/imperium-logo.png';
import Logos from './Logos';

const FormCadastro = () => {
    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [confirmarSenha, setConfirmarSenha] = useState('');  // Adiciona um estado para confirmar a senha
    const [erro, setErro] = useState('');  // Adiciona um estado para exibir mensagens de erro

    const handleSubmit = async (e) => {
      e.preventDefault();
      
      if (senha !== confirmarSenha) {
        setErro('As senhas não coincidem.');
        return;
      }

      try {
        const response = await axios.post('http://localhost:5050/api/Usuarios/cadastro', {
          nome,
          email,
          senha
        });
        alert('Cadastro realizado com sucesso!');
        // Redirecionar ou limpar o formulário, se necessário
      } catch (error) {
        console.error('Erro ao cadastrar usuário:', error);
        setErro('Erro ao realizar o cadastro. Tente novamente.');
      }
    };

    return (
      <form className='form-cadastro' onSubmit={handleSubmit}>
        <img src={logo} alt="Logo da Imperium" className='logo-cadastro' />
        <h1>CADASTRO</h1>
        <div className='texto-logar'>
            <p>Já possui cadastro?</p>
            <div className='lado-a-lado'>
                <p>Pode entrar no sistema clicando</p>
                <Link className='aqui' to='/login'>aqui!</Link>
            </div>
        </div>
        <div className='lado-a-lado'>
           <FaUser className='label-icon' /> 
          <label>Nome</label>
        </div>
        <input
          className='input-form'
          type="text"
          placeholder="Seu nome completo"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          required
        />
        <div className='lado-a-lado'>
          <FaEnvelope className='label-icon' />
          <label>E-mail</label>
        </div>
        <input
          className='input-form'
          type="email"
          placeholder="Seu e-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <div className='lado-a-lado'>
          <FaLock className='label-icon' />
          <label>Senha</label>
        </div>
        <input
          className='input-form'
          type="password"
          placeholder="Insira sua senha"
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
          required
        />
        <div className='lado-a-lado'>
          <FaLock className='label-icon' />
          <label>Confirme sua senha</label>
        </div>
        <input
          className='input-form'
          type="password"
          placeholder="Insira sua senha novamente"
          value={confirmarSenha}
          onChange={(e) => setConfirmarSenha(e.target.value)}
          required
        />
        {erro && <p className='erro'>{erro}</p>}  {/* Exibe mensagem de erro, se houver */}
        <button className='button-cadastro' type="submit">CADASTRAR</button>
        <Logos />
      </form>
    );
};

export default FormCadastro;
